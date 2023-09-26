import socket
import threading
import time

# Dictionary to store client names and their respective sockets
clients = {}

# Function to broadcast messages to all connected clients
def broadcast(message, client_socket):
    for client in clients:
        if client != client_socket:
            try:
                client.send(message)
            except:
                # Remove the client from the dictionary if it's not reachable
                remove_client(client)

# Function to handle incoming client connections
def handle_client(client_socket):
    # Ask for the user's name
    client_socket.send("Enter your name: ".encode())
    name = client_socket.recv(1024).decode()
    welcome_message = f"Welcome, {name}!"
    client_socket.send(welcome_message.encode())

    # Broadcast the new user's arrival to other clients
    broadcast(f"{name} has joined the chat.\n".encode(), client_socket)

    while True:
        try:
            message = client_socket.recv(1024)
            if message:
                timestamp = time.strftime("[%Y-%m-%d %H:%M:%S] ")
                broadcast(timestamp.encode() + f"{name}: ".encode() + message, client_socket)
        except:
            # Remove the client if an error occurs
            remove_client(client_socket)
            break

# Function to remove a client from the server
def remove_client(client_socket):
    if client_socket in clients:
        client_name = clients[client_socket]
        broadcast(f"{client_name} has left the chat.\n".encode(), client_socket)
        del clients[client_socket]

# Main server code
def main():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(("0.0.0.0", 1025))  # Change the IP and port as needed
    server.listen(5)

    print("Server is listening for connections...")

    while True:
        client_socket, client_addr = server.accept()
        print(f"Accepted connection from {client_addr}")
        clients[client_socket] = ""
        client_thread = threading.Thread(target=handle_client, args=(client_socket,))
        client_thread.start()

if __name__ == "__main__":
    main()
