"use strict";

const API_PRODUCTS = "https://localhost:7119/api/products";
const API_ORDERS = "https://localhost:7119/api/orders";
const messageElement = document.getElementById("message");
const TBODY_ALL_ORDERS = "listAllOrders";
const TBODY_ORDER_BY_ID = "listOrderById";


let quantityOfProducts=0
function showMessage(message, isError = false) {
    messageElement.textContent = message;
    messageElement.className = isError ? "Error" : "sucess";
}
function formatPice(value) {
    return Number(value).toFixed(2);
}

async function getJsonFromOrderServer(url) {
    let response;

    try {
        response = await fetch(url, {
            method: "GET",
            headers: {
                "Accept": "application/json"
            }
        });
    } catch {
        throw new Error("unable to comunicate with the API server");
    }

    if (!response.ok) {
        switch (response.status) {
            case 400:
                throw new Error("Invalid request parameters");
            case 404:
                throw new Error("No matching product found");
            case 500:
                throw new Error("The server could not complete the request");
            default:
                throw new Error("The request could not be completed");
        }
    }
    try {
        return await response.json();
    } catch {
        throw new Error("The server returned an invalid response");
    }
}



async function postJsonFromOrderServer(url,order) {
    let response;
    try {
        response = await fetch(url, {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            },
            body: JSON.stringify(order)


        });
    } catch {
        throw new Error("Unable to comunicate with the API server");
    }
    if (!response.ok) {
        switch (response.status) {
            case 400:
                throw new Error("Invalid request parameters");
            case 404:
                throw new Error("No matching product found");
            case 409:
                throw new Error("This order already exists");
            case 500:
                throw new Error("The server could not complete the request");
            default:
                throw new Error("The request could not be completed");
        }
    }
    return true;
}

async function loadAllProducts() {
   
    try {
        const products = await getJsonFromOrderServer(API_PRODUCTS);
        displayProducts(products);
        showMessage(`${ products.length } product(s) loaded successfully.`);
    }
    catch (error) {
        document.getElementById("productsTable").replaceChildren
        showMessage(error.message, true);
    }
}

async function createOrder() {
    event.preventDefault();
    const idOrder = Number(document.getElementById("orderId").value)
    const name = document.getElementById("consumer").value

    let listProducts = [];

    for (let i = 0; i < quantityOfProducts; i++) {
        let id = Number(document.getElementById(`productID-${i}`).value)
        let quantity = Number (document.getElementById(`Quantity-${i}`).value);
        listProducts.push({
            IdProduct: id ,
            quantity: quantity
        });

    }

    let order = {
        Id: idOrder,
        Name: name,
        orderProducts: listProducts
    };

    try {
        await postJsonFromOrderServer(API_ORDERS, order);
        alert("Order sent");
    } catch (error) {
        alert("Error: " + error.message);
    } finally {
        const div = document.getElementById("addProducts");
        div.replaceChildren();
    }
}

function displayProducts(products) {
    const table = document.getElementById("productsTable")
    for (const product of products) {
        const row = document.createElement("tr");
        const values = [
            product.id,
            product.name,
            product.price
        ];
        for (const value of values) {
            const cell = document.createElement("td");
            cell.textContent = value;
            row.appendChild(cell)
        }
        table.appendChild(row)
    }
}



function addRowstoProducts() {
    const div = document.getElementById("addProducts");

    let newRow =`<div style="margin-top: 15px;">
                <label> Product id</label>
                <input  id="productID-${quantityOfProducts}"type="number" min="1" step="1" required />
                <label> Quantity</label>
                <input  id="Quantity-${quantityOfProducts}"type="number" min="1" step="1" required />
                </div>`;
    div.insertAdjacentHTML('beforeend', newRow);
    quantityOfProducts++;

}

async function showAllOrders() {
    try {
        const response = await getJsonFromOrderServer(API_ORDERS);
        displayOrders(response, TBODY_ALL_ORDERS);
        showMessage(`${response.length} orders(s) loaded successfully.`);
    } catch (error) {
        document.getElementById("allOrdersTable").replaceChildren
        showMessage(error.message, true)
    }
}

function displayOrders(Orders, targetBodyId) {
    const table = document.getElementById(targetBodyId);
    for (const order of Orders){
        const row = document.createElement("tr");
        const values= [
            order.id,
            order.name,
            order.date,
            order.total,
            order.totalOfProducts
        ]
        for (const value of values) {
            const cell = document.createElement("td");
            cell.textContent = value;
            row.appendChild(cell)
        }
        
        const actionButton = document.createElement("td");
        actionButton.innerHTML = `<button type="button" id="order${order.id}"> Details</button>`

        const btn = actionButton.querySelector("button")
        btn.onclick = () => {
            const linhaEscondida  = document.getElementById(`hidden-${targetBodyId}-${order.id}`);
            if (linhaEscondida.style.display === "none") {
                linhaEscondida.style.display = "table-row"; 
                const hiddenTable =`<table >
                                        <thead>
                                            <tr>
                                                <th> name</th>
                                                <th> quantity</th>
                                                <th> unit price</th>
                                            </tr>
                                        </thead>
                                <tbody id="ProductsInOrder-${targetBodyId}-${order.id}"></tbody>
                                </table>`
                linhaEscondida.innerHTML = `<td colspan="6">${hiddenTable}</td>`;
                addProductDetails(order.orderProducts,order.id,targetBodyId);
            } else {
                linhaEscondida.style.display = "none";
            }
        }
        row.appendChild(actionButton);
        table.appendChild(row);
        table.appendChild(hiddenLine(order,targetBodyId))
        
    }
}

function addProductDetails(products, id, targetBodyId) {
    const table =  document.getElementById(`ProductsInOrder-${targetBodyId}-${id}`);
    for (const product of products) {
        const row = document.createElement("tr");
        const values = [
            product.name,
            product.quantity,
            product.price
        ];
        for (const value of values) {
            const cell = document.createElement("td");
            cell.textContent = value;
            row.appendChild(cell)
        }
        table.appendChild(row)
    }
}

function hiddenLine(order, targetBodyId) {
    const hidden = document.createElement("tr");
    hidden.id = `hidden-${targetBodyId}-${order.id}`
    hidden.style.display = "none";

    const productCell = document.createElement("td")
    productCell.colSpan = 6;

    hidden.appendChild(productCell)
    return hidden;
}

async function getOrderById(event) {
    event.preventDefault();
    const id = document.getElementById("searchOrderId").value

    try {
        const order = await getJsonFromOrderServer(`${API_ORDERS}/${id}`);
        document.getElementById(TBODY_ORDER_BY_ID).replaceChildren();

        displayOrders([order], TBODY_ORDER_BY_ID);
    }
    catch (error) {
        document.getElementById(TBODY_ORDER_BY_ID).replaceChildren();
        showMessage(error.message, true);
    }
}



document.getElementById("addRows").addEventListener("click", addRowstoProducts);
document.getElementById("createOrder").addEventListener("submit", createOrder);
document.getElementById("showAllOrders").addEventListener("click", showAllOrders);
document.getElementById("showOrderById").addEventListener("click", getOrderById );
loadAllProducts();

