import sys
import imaplib
import email
import re
from email.header import decode_header
import json
from concurrent.futures import ThreadPoolExecutor, as_completed
from threading import Lock
from collections import defaultdict
from datetime import datetime, timedelta

def delete_emails(emailAddress, imap, line_dict, permanently_delete):
    print(f"Deleting {line_dict[emailAddress]} Emails for {emailAddress}")
    provider = emailAddress.strip()
    count = 1
    status, messages = imap.search(None, f'FROM "{provider}"')
    trashFolder = '[Gmail]/Trash'
    if messages:
        messages = messages[0].split()
        messageCount = len(messages)

        print(permanently_delete)
        if permanently_delete:
            for mail in messages:
                imap.store(mail, '+X-GM-LABELS', '\\Trash')
                percentage = float("{0:.1f}".format(count / messageCount * 100))
                print(f"{count} email(s) out of {messageCount} deleted, {percentage}%")
                count += 1

            imap.select(trashFolder)
            imap.store("1:*", '+FLAGS', '\\Deleted')
            imap.expunge()
        else:
            for mail in messages:
                imap.store(mail, '+X-GM-LABELS', '\\Trash')
                percentage = float("{0:.1f}".format(count / messageCount * 100))
                print(f"{count} email(s) out of {messageCount} deleted, {percentage}%")
                count += 1
        imap.expunge()

        print("All selected mails have been deleted")
    else:
        print("No messages found for deletion.")

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


def search_emails(emails, imap):
    line_dict = {}
    for emailAddress in emails:
        emailRec = emailAddress.strip()
        print(f"searching for {emailRec}")
        status, messages = imap.search(None, f'FROM "{emailRec}"')
        # convert messages to a list of email IDs
        messages = messages[0].split()
        line_dict[emailAddress] = len(messages)
    return line_dict

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
                    email_counts[sender_email] += 1
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
    email_counts = defaultdict(int)
    lock = Lock()  # Lock to synchronize access to email_counts and IMAP object

    try:
        status, data = imap.search(None, 'ALL')
        email_numbers = data[0].split()[:500]  # Limit to the first 100 emails
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

def scan_emails_with_time(imap):
    email_counts = defaultdict(int)
    lock = Lock()  # Lock to synchronize access to email_counts and IMAP object

    try:
        today = datetime.today()
        days_ago_7 = today - timedelta(days=14)
        search_date = days_ago_7.strftime('%d-%b-%Y')

        status, data = imap.search(None, f'(SINCE "{search_date}")')
        email_numbers = data[0].split()[:500]  # Limit to the first 500 emails
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
        scan_emails_with_time(imap)
        #scan_emails(imap)

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
