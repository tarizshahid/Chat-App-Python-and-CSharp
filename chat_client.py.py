import socket
import threading

def receive_messages(client_socket):
    while True:
        try:
            message = client_socket.recv(1024).decode()
            if message:
                print(message)
        except:
            print("Connection to the server has been lost.")
            client_socket.close()
            break

def main():
    server_ip = input("Enter the server IP address: ")
    server_port = int(input("Enter the server port: "))

    client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client_socket.connect((server_ip, server_port))

    name = input("Enter your name: ")
    client_socket.send(name.encode())

    receive_thread = threading.Thread(target=receive_messages, args=(client_socket,))
    receive_thread.start()

    while True:
        message = input()
        if message:
            client_socket.send(message.encode())

if __name__ == "__main__":
    main()
