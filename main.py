from socket import *
## Create socket - the endpoint of a link between programs running over a network
host = "127.0.0.1"
port = 5000

# Accept connection
s = socket()
s.bind((host, port)) # bind the socket to the address and port to listen to 
print("Searching!")
s.listen(1) # socket is now listening for new connections - if 1 connection is refused, the socket will refuse all future connections

conn, addr = s.accept()
print(f"Connection from {addr}")
# When it recieves a message (chance of rain), print it
while True:
    # data = conn.recv(1024).decode()
    data = int.from_bytes(conn.recv(1024))
    if not data:
        break

    print(f"Recieved: {data}")