import sys
import imaplib
import email
import re
from email.header import decode_header
import json
from concurrent.futures import ThreadPoolExecutor, as_completed
from threading import Lock
import os
from urllib.parse import unquote

def search_emails(emails, imap):
    line_dict = {}

    for emailAddress in emails:
        emailRec = emailAddress.strip()
        print(f"searching for {emailRec}")
        messages = imap.search(None, f'FROM "{emailRec}"')

        # convert messages to a list of email IDs
        messages = messages[0].split()
        line_dict[emailAddress] = len(messages)

    return line_dict

def delete_emails(emailAddress, imap, line_dict):
    print(f"Deleting {line_dict[emailAddress]} Emails for {emailAddress}")
    provider = emailAddress.strip()
    count = 1
    messages = imap.search(None, f'FROM "{provider}"')
    messages = messages[0].split()
    messageCount = len(messages)

    for mail in messages:
        # mark the mail as deleted
        imap.store(mail, '+X-GM-LABELS', '\\Trash')
        percentage = float("{0:.1f}".format(count / messageCount * 100))
        print(f"{count} email(s) out of {messageCount} deleted, {percentage}%")
        count += 1
    print("All selected mails have been deleted")
    # delete all the selected messages
    imap.expunge()

def close_connection(imap):
    # close the mailbox
    imap.close()
    # logout from the server
    imap.logout()

def fetch_raw_email(imap, num):
    status, msg_data = imap.fetch(num, '(RFC822)')
    if status == 'OK':
        raw_email = msg_data[0][1]
        try:
            # Decode the raw email data using 'utf-8' encoding
            raw_email_string = raw_email.decode('utf-8')
        except UnicodeDecodeError:
            # If 'utf-8' decoding fails, try a different encoding
            raw_email_string = raw_email.decode('latin-1')
        return raw_email_string
    return None

def extract_sender_email(raw_email_string):
    msg = email.message_from_string(raw_email_string)
    sender_email = msg['From']
    # Use regular expressions to extract the email address from the sender field
    matches = re.findall(r'[\w\.-]+@[\w\.-]+', sender_email)
    if matches:
        return matches[0]
    return None

def search_and_delete_emails(emails, imap):
    line_dict = search_emails(emails, imap)

    for emailAddress, count in line_dict.items():
        if count < 1:
            print(f"No Emails for {emailAddress}")
        else:
            delete_emails(emailAddress, imap, line_dict)

import sys
import imaplib
import email
import re
from email.header import decode_header

def search_emails(emails, imap):
    line_dict = {}

    for emailAddress in emails:
        emailRec = emailAddress.strip()
        print(f"searching for {emailRec}")
        messages = imap.search(None, f'FROM "{emailRec}"')

        # convert messages to a list of email IDs
        messages = messages[0].split()
        line_dict[emailAddress] = len(messages)

    return line_dict

def delete_emails(emailAddress, imap, line_dict, permanently_delete):
    print(f"Deleting {line_dict[emailAddress]} Emails for {emailAddress}")
    provider = emailAddress.strip()
    count = 1
    messages = imap.search(None, f'FROM "{provider}"')
    messages = messages[0].split()
    messageCount = len(messages)

    for mail in messages:
        if permanently_delete:
            # permanently delete the email
            imap.store(mail, '+FLAGS', '\\Deleted')
        else:
            # move the email to the trash folder
            imap.store(mail, '+X-GM-LABELS', '\\Trash')
        percentage = float("{0:.1f}".format(count / messageCount * 100))
        print(f"{count} email(s) out of {messageCount} deleted, {percentage}%")
        count += 1

    print("All selected mails have been deleted")
    if permanently_delete:
        # permanently remove the deleted messages
        imap.expunge()

def close_connection(imap):
    # close the mailbox
    imap.close()
    # logout from the server
    imap.logout()

def fetch_raw_email(imap, num):
    status, msg_data = imap.fetch(num, '(RFC822)')
    if status == 'OK':
        raw_email = msg_data[0][1]
        try:
            # Decode the raw email data using 'utf-8' encoding
            raw_email_string = raw_email.decode('utf-8')
        except UnicodeDecodeError:
            # If 'utf-8' decoding fails, try a different encoding
            raw_email_string = raw_email.decode('latin-1')
        return raw_email_string
    return None

def extract_sender_email(raw_email_string):
    msg = email.message_from_string(raw_email_string)
    sender_email = msg['From']
    # Use regular expressions to extract the email address from the sender field
    matches = re.findall(r'[\w\.-]+@[\w\.-]+', sender_email)
    if matches:
        return matches[0]
    return None

def search_and_delete_emails(emails, imap, permDelete=False):
    line_dict = search_emails(emails, imap)

    for emailAddress, count in line_dict.items():
        if count < 1:
            print(f"No Emails for {emailAddress}")
        else:
            delete_emails(emailAddress, imap, line_dict, permDelete)

def process_email(imap, email_counts, num, lock, processed_emails, total_emails):
    try:
        with lock:
            raw_email_string = fetch_raw_email(imap, num)
        if raw_email_string:
            sender_email = extract_sender_email(raw_email_string)
            if sender_email:
                with lock:
                    if sender_email in email_counts:
                        email_counts[sender_email] += 1
                    else:
                        email_counts[sender_email] = 1
                    processed_emails[0] += 1
                    percentage = (processed_emails[0] / total_emails) * 100
                    print(f"Processing email {processed_emails[0]}/{total_emails} ({percentage:.2f}% complete)")
    except Exception as e:
        print(f"An error occurred while processing email {num.decode()}: {e}")
        with lock:
            response = getattr(imap, 'response', None)
        if response:
            print(f"Server response: {response}")

def scan_emails(imap):
    email_counts = {}  # Dictionary to store email addresses and their counts
    lock = Lock()  # Lock to synchronize access to email_counts and IMAP object

    try:
        data = imap.search(None, 'ALL')
        email_numbers = data[0].split()  # Limit to the first 100 emails
        total_emails = len(email_numbers)  # Total number of emails to process
        print(f"Total emails to process: {total_emails}")

        processed_emails = [0]  # Counter for processed emails

        with ThreadPoolExecutor() as executor:
            futures = []
            for num in email_numbers:
                future = executor.submit(process_email, imap, email_counts, num, lock, processed_emails, total_emails)
                future.num = num  # Attach email number to future object
                futures.append(future)

            for future in as_completed(futures):
                num = future.num  # Retrieve email number from future object
                try:
                    future.result()
                except Exception as e:
                    print(f"An error occurred while processing email {num.decode()}: {e}")
                    with lock:
                        response = getattr(imap, 'response', None)
                    if response:
                        print(f"Server response: {response}")

        # Export email addresses and counts as JSON
        with open('email_counts.json', 'w') as file:
            json.dump(email_counts, file)

    except Exception as e:
        print(f"An error occurred while scanning emails: {e}")


def scan_emails_unsub(imap):
    subLinksFileName = 'unsubscribeLinks'
    limit = 0
    emailCount = 0
    unsubLinks = []

    subLinksFile = open(subLinksFileName, 'w')

    rv, data = imap.search(None, "TEXT unsubscribe")

    for num in data[0].split():
        try:
            if limit != 0 and emailCount > limit:
                print('Reached the maximum of emails we want to process')
                break

            print('Processing email id = ', num, '...', sep='')

            rv, data = imap.fetch(num, '(RFC822)')
            if rv != 'OK':
                print("ERROR getting message", num)
                continue

            msg = email.message_from_bytes(data[0][1])
            sender_email = msg['From']

            if msg.is_multipart():
                for payload in msg.get_payload():
                    body = payload.get_payload()
            else:
                body = msg.get_payload()

            # Extract the unsubscribe link using regular expressions
            unsubscribe_links = re.findall(r'href="(.*?)"', body)
            for link in unsubscribe_links:
                if 'unsubscribe' in link:
                    decoded_link = unquote(link)
                    if decoded_link not in unsubLinks:
                        unsubLinks.append(decoded_link)
                        print('Found new link to unsubscribe from: ', decoded_link)
                        subLinksFile.write(f"Sender: {sender_email}\nUnsubscribe Link: {decoded_link}\n\n")

        except Exception as e:
            print('We encountered an exception but we decided to keep going.')
            print('The exception was: ', e)
            pass

        emailCount += 1

    print('Emails processed: ', emailCount)
    print('Unsubscribe links found: ', len(unsubLinks))

    subLinksFile.close()

def main(username, password, method, permDelete=False):
    # create an IMAP4 class with SSL
    imap = imaplib.IMAP4_SSL("imap.gmail.com", port=993)
    # authenticate
    imap.login(username, password)
    print(username)
    print(password)
    # select the mailbox I want to delete in
    # if you want SPAM, use imap.select("SPAM") instead
    imap.select('"[Gmail]/All Mail"')

    if method == "delete":
        with open("emailDeleteList.txt") as file_in:
            emails = file_in.readlines()
        search_and_delete_emails(emails, imap, permDelete)

    if method == "scan":
        scan_emails(imap)

    if method == "unsubscribe":
        scan_emails_unsub(imap)

    close_connection(imap)

    print("End")

if __name__ == "__main__":
    if len(sys.argv) < 4:
        print("Please provide both username and password as command-line arguments also a method.")
    else:
        username = sys.argv[1]
        password = sys.argv[2]
        method = sys.argv[3]
        permDelete = sys.argv[4] if len(sys.argv) > 4 else None
        main(username, password, method, permDelete)
