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
    status, data = imap.search(None, 'ALL')

    if(method == "delete"):
        with open("emailDeleteList.txt") as file_in:
            emails = file_in.readlines()

        line_dict = search_emails(emails, imap)

        for emailAddress, count in line_dict.items():
            if count < 1:
                print(f"No Emails for {emailAddress}")
            else:
                delete_emails(emailAddress, imap, line_dict)

    if method == "scan":
        email_addresses = set()  # Use a set to store unique email addresses
        for num in data[0].split():
            # Fetch the email message
            status, msg_data = imap.fetch(num, '(RFC822)')
            if status == 'OK':
                raw_email = msg_data[0][1]
                try:
                    # Decode the raw email data using 'utf-8' encoding
                    raw_email_string = raw_email.decode('utf-8')
                except UnicodeDecodeError:
                    # If 'utf-8' decoding fails, try a different encoding
                    raw_email_string = raw_email.decode('latin-1')

                # Parse the raw email string
                msg = email.message_from_string(raw_email_string)
                # Extract the email address of the sender
                sender_email = msg['From']
                print(f"found email from {sender_email}")
                # Use regular expressions to extract the email address from the sender field
                matches = re.findall(r'[\w\.-]+@[\w\.-]+', sender_email)
                if matches:
                    email_addresses.add(matches[0])
        
        with open('email_addresses.txt', 'w') as file:
            file.write('\n'.join(email_addresses))


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
