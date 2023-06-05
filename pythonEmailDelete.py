import sys
import imaplib
import email
import re
from email.header import decode_header
import json

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

def delete_emails(emailAddress, imap, line_dict):
    print(f"Deleting {line_dict[emailAddress]} Emails for {emailAddress}")
    provider = emailAddress.strip()
    count = 1
    status, messages = imap.search(None, f'FROM "{provider}"')
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
        status, messages = imap.search(None, f'FROM "{emailRec}"')

        # convert messages to a list of email IDs
        messages = messages[0].split()
        line_dict[emailAddress] = len(messages)

    return line_dict

def delete_emails(emailAddress, imap, line_dict):
    print(f"Deleting {line_dict[emailAddress]} Emails for {emailAddress}")
    provider = emailAddress.strip()
    count = 1
    status, messages = imap.search(None, f'FROM "{provider}"')
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

def scan_emails(imap):
    email_counts = {}  # Dictionary to store email addresses and their counts

    # List all the mailbox categories
    status, mailbox_list = imap.list()
    if status == 'OK':
        print("Mailbox Categories:")
        for mailbox in mailbox_list:
            mailbox_name = mailbox.decode().split(' "/" ')[-1]  # Extract the category name
            print(mailbox_name)

    # List special folders like "Promotions" and "Updates"
    status, special_folders = imap.list(pattern='*')
    if status == 'OK':
        print("Special Folders:")
        for folder in special_folders:
            folder_name = folder.decode().split(' "/" ')[-1]  # Extract the folder name
            print(folder_name)

    status, data = imap.search(None, 'ALL')
    total_emails = len(data[0].split())  # Total number of emails found
    print(f"Total emails found: {total_emails}")

    processed_emails = 0

    for index, num in enumerate(data[0].split(), start=1):
        raw_email_string = fetch_raw_email(imap, num)
        if raw_email_string:
            sender_email = extract_sender_email(raw_email_string)
            if sender_email:
                processed_emails += 1
                percentage = (processed_emails / total_emails) * 100
                print(f"Processing email {index}/{total_emails} ({percentage:.2f}% complete)")
                if sender_email in email_counts:
                    email_counts[sender_email] += 1
                else:
                    email_counts[sender_email] = 1
        if index == 200:
            break

    # Export email addresses and counts as JSON
    with open('email_counts.json', 'w') as file:
        json.dump(email_counts, file)

def main(username, password, method):
    # create an IMAP4 class with SSL
    imap = imaplib.IMAP4_SSL("imap.gmail.com")
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
        search_and_delete_emails(emails, imap)

    if method == "scan":
        scan_emails(imap)

    close_connection(imap)

    print("End")

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Please provide both username and password as command-line arguments.")
    else:
        username = sys.argv[1]
        password = sys.argv[2]
        method = sys.argv[3]
        main(username, password, method)
