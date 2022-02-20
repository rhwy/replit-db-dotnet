# Replit Database
is an easy to use key-value store.

## COMMANDS
We've set an environment variable called REPLIT_DB_URL in your repl.

- **Set** a key to a value
`curl $REPLIT_DB_URL -d {key}={value}`
- **Get** a key's value
`curl $REPLIT_DB_URL/{key}`
- **Delete** a key
`curl -XDELETE $REPLIT_DB_URL/{key}`
- **List** all keys with the prefix {key}
`curl "$REPLIT_DB_URL?prefix={key}"`

You can use the Database from any repl by making HTTP requests similar to those above. We have language clients for Python, Go, and Node, switch to a file that uses that language to see how to use the client.

## Compatibility

In order to ensure coherence between drivers implementations we'll follow the signature of Javascript's client:

```ts 
export class Client {
  constructor(key?: string);

  // Native
  public get(key: string, options?: { raw?: boolean }): Promise<unknown>;
  public set(key: string, value: any): Client;
  public delete(key: string): Client;
  public list(prefix?: string): Promise<string[]>;

  // Dynamic
  public empty(): Client;
  public getAll(): Record<any, any>;
  public setAll(obj: Record<any, any>): Client;
  public deleteMultiple(...args: string[]): Client;
}
```