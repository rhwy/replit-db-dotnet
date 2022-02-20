# Replit Database
is an easy to use key-value store.

## COMMANDS
We've set an environment variable called REPLIT_DB_URL in your repl.

- **Set** a key to a value
`curl $REPLIT_DB_URL -d {key}={value}`
- **Get** a key's value
`curl $REPLIT_DB_URL {key}`
- **Delete** a key
`curl -XDELETE $REPLIT_DB_URL {key}`
- **List** all keys
`curl "$REPLIT_DB_URL?prefix={prefix}`

You can use the Database from any repl by making HTTP requests similar to those above. We have language clients for Python, Go, and Node, switch to a file that uses that language to see how to use the client.