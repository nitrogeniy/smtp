# smtp
Simple MailKit-based mailer.

Usage: 'smtp --param value'; * = required.
user:       * username
password:   password
server:     * server (smtp.*)
from:       source email; by def: user
fromfield:  'from' field; by def: from
to:         * dest email
tofield:    'to' field; by def: to
port:       * port
subject:    subj
body:       body
dry:        don't send, do test login
skipconn:   skip connection; 'do nothing' switch