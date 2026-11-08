import Order_pb2
import Order_pb2_grpc
import grpc
from concurrent import futures
class Order(Order_pb2_grpc.OrderServiceServicer):
    def CalculateTotal(self, request, context):
       subtotal=0
       if not request.items:
           context.abort(grpc.StatusCode.INVALID_ARGUMENT,"A encomenda tem de ter produtos")
       for item in request.items:
           if item.price<=0 or item.quantity<=0:
               context.abort(grpc.StatusCode.INVALID_ARGUMENT,"A quantidade e o preço tem de ser maiores que 0")
           subtotal+=(item.price*item.quantity)
       subtotal=round(subtotal,2)
       discount=0
       if(subtotal>=100):
           discount=subtotal*.1
           discount=round(discount,2)
       total=subtotal-discount
       total=round(total,2)
       return Order_pb2.OrderResponse(total=total,subTotal=subtotal,discount=discount)


def server():
    server=grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    Order_pb2_grpc.add_OrderServiceServicer_to_server(Order(),server)

    server.add_insecure_port("[::]:10000")

    server.start()
    print("The server will start")

    server.wait_for_termination()

if __name__=="__main__":
    server()