from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class OrderRequest(_message.Message):
    __slots__ = ("name", "price", "quantity")
    NAME_FIELD_NUMBER: _ClassVar[int]
    PRICE_FIELD_NUMBER: _ClassVar[int]
    QUANTITY_FIELD_NUMBER: _ClassVar[int]
    name: str
    price: float
    quantity: int
    def __init__(self, name: _Optional[str] = ..., price: _Optional[float] = ..., quantity: _Optional[int] = ...) -> None: ...

class OrderResponse(_message.Message):
    __slots__ = ("total", "subTotal", "discount")
    TOTAL_FIELD_NUMBER: _ClassVar[int]
    SUBTOTAL_FIELD_NUMBER: _ClassVar[int]
    DISCOUNT_FIELD_NUMBER: _ClassVar[int]
    total: float
    subTotal: float
    discount: float
    def __init__(self, total: _Optional[float] = ..., subTotal: _Optional[float] = ..., discount: _Optional[float] = ...) -> None: ...
