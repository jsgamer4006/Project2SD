import Order_pb2
import Order_pb2_grpc

class Order(Order_pb2_grpc.OrderServiceServicer):
    def CalculateTotal(self, request, context):
        name=request.name
        price=request.price
        quantity=request.quantity
        if not name.strip() :
            context.abort(grpc.StatusCode.INVALID_ARGUMENT,"Please enter a name")
        if price <=0:
            context.abort(grpc.StatusCode.INVALID_ARGUMENT,"Please enter a valid price")
        if quantity <=0 :
            context.abort(grpc.StatusCode.INVALID_ARGUMENT,"Please enter a valid quantity")
        subtotal=round(price*quantity)

        discount=0
        if(subtotal>=100):
            discount=subtotal*0.1
        total=subtotal-discount
        total=round(total,2)
        return Order_pb2.OrderResponse(total=total,subTotal=subtotal,discount=discount)
        