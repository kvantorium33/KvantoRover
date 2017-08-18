import SocketServer
import serial
import time

serialport = serial.Serial(port="/dev/ttyACM0", baudrate=57600, bytesize=8, timeout=3)

class MyTCPHandler(SocketServer.BaseRequestHandler):

    def handle(self):
        # self.request is the TCP socket connected to the client
        while True:
            self.data = self.request.recv(1024).strip()
            print "{} wrote:".format(self.client_address[0])
            print self.data
            # just send back the same data, but upper-cased
            serialport.write(self.data)
            print "Robot answer:"
            while True:
                resp = serialport.readline()
                print resp
                self.request.sendall(resp)
                if(resp == "Robot is OK!\n")
                    break

if __name__ == "__main__":
    HOST, PORT = "0.0.0.0", 9999

    # Create the server, binding to localhost on port 9999
    server = SocketServer.TCPServer((HOST, PORT), MyTCPHandler)

    # Activate the server; this will keep running until you
    # interrupt the program with Ctrl-C
    server.serve_forever()
