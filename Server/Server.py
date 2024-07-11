import socket
import threading
import time

server_ip = "localhost"
port = 9999

class Game:
    def __init__(self, player1, player2):
        self.player1 = player1
        self.player2 = player2
        self.ready = False
        self.replay_requests = {player1: False, player2: False}
        self.create_game()
        self.start_game()

    def create_game(self):
        try:
            if self.is_socket_valid(self.player1):
                self.player1.send("[x]".encode("utf-8"))
            if self.is_socket_valid(self.player2):
                self.player2.send("[o]".encode("utf-8"))
        except Exception as e:
            print(f"Error sending signal: {e}")

    def start_game(self):
        self.ready = True
        try:
            if self.is_socket_valid(self.player1):
                self.player1.send("[GAME STARTED]".encode("utf-8"))
            if self.is_socket_valid(self.player2):
                self.player2.send("[GAME STARTED]".encode("utf-8"))

            threading.Thread(target=self.handle_player, args=(self.player1, self.player2)).start()
            threading.Thread(target=self.handle_player, args=(self.player2, self.player1)).start()
        except Exception as e:
            print(f"Error starting game: {e}")

    def handle_player(self, player, opponent):
        while self.ready:
            try:
                request = player.recv(1024).decode("utf-8")
                if not request:
                    continue

                if request == "[DISCONNECT]" or request == "[CLOSEAPP]":
                    print(f"Request from {player.getpeername()}: {request}")
                    opponent.send("[OPPONENT DISCONNECTED]".encode("utf-8"))

                    break

                elif request.startswith("[SPAWN"):
                    print(f"Request from {player.getpeername()}: {request}")

                    if self.is_socket_valid(opponent):
                        opponent.send(request.encode("utf-8"))

                elif request == "[REPLAY REQUEST]":
                    print(f"Request from {player.getpeername()}: {request}")
                    self.replay_requests[player] = True

                    if self.replay_requests[opponent]:
                        self.restart_game()
                    else:
                        if self.is_socket_valid(player):
                            player.send(request.encode("utf-8"))

                elif request == "[EXIT]":
                    print(f"Request from {player.getpeername()}: {request}")

                    opponent.send("[EXIT]".encode("utf-8"))
                    player.send("[EXIT]".encode("utf-8"))

                    if self.replay_requests[opponent]:
                        opponent.send("[OPPONENT DISCONNECTED]".encode("utf-8"))
                
                    break

            except:
                break

        self.end_game(player, opponent)

    def restart_game(self):
        self.replay_requests = {self.player1: False, self.player2: False}
        try:
            if self.is_socket_valid(self.player1):
                self.player1.send("[GAME RESTARTED]".encode("utf-8"))
            if self.is_socket_valid(self.player2):
                self.player2.send("[GAME RESTARTED]".encode("utf-8"))
            print("Game Restarted!")
        except Exception as e:
            print(f"Error restarting game: {e}")

    def end_game(self, player, opponent):
        self.ready = False
        print("Game Ended!")

        try:
            with active_games_lock:
                if self in active_games:
                    active_games.remove(self)

            if self.is_socket_valid(opponent):
                opponent.shutdown(socket.SHUT_RDWR)
                opponent.close()

            if self.is_socket_valid(player):
                player.shutdown(socket.SHUT_RDWR)
                player.close()

        except Exception as e:
            print(f"Error ending game: {e}")

    def is_socket_valid(self, sock):
        try:
            # Check if the socket file descriptor is valid and attempt a non-blocking send
            sock.send(b"")
            return sock.fileno() != -1
        except:
            return False

def run_server():
    try:
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        server.bind((server_ip, port))
        server.listen()
        print("Server Started. Waiting for connection!")

        while True:
            try:
                client_socket, addr = server.accept()
                print(f"Accepted connection from {addr[0]}:{addr[1]}")

                if is_socket_valid(client_socket):
                    client_socket.send("[Connected to server]".encode("utf-8"))

                with waiting_clients_lock:
                    waiting_clients.append(client_socket)

                    if len(waiting_clients) == 1:
                        threading.Thread(target=monitor_waiting_client).start()
                        if is_socket_valid(client_socket):
                            client_socket.send("[WAITING]".encode("utf-8"))
                        print("Waiting for one more connection")

                    elif len(waiting_clients) >= 2:
                        player2 = waiting_clients.pop()
                        player1 = waiting_clients.pop()

                        print("Creating a new game...")
                        game = Game(player1, player2)
                        with active_games_lock:
                            active_games.append(game)

            except Exception as e:
                print(f"Error accepting connections: {e}")

    except Exception as e:
        print(f"Error setting up server: {e}")
    finally:
        server.close()

def monitor_waiting_client():
    global waiting_clients
    while True:
        with waiting_clients_lock:
            if len(waiting_clients) == 1:
                client = waiting_clients[0]
                if not is_socket_valid(client):
                    waiting_clients.pop()
                    print("Client cancel waiting")
                    break
            else:
                break
        time.sleep(0.1)

def is_socket_valid(sock):
    try:
        sock.send("[]".encode("utf-8"))
        return sock.fileno() != -1
    except:
        return False

waiting_clients = []
active_games = []
waiting_clients_lock = threading.Lock()
active_games_lock = threading.Lock()

run_server()
