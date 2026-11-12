"use strict";

const API_PRODUCTS = "https://localhost:7119/api/products";
const API_ORDERS = "https://localhost:7119/api/orders";
const messageElement = document.getElementById("message");
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
        await postJsonFromOrderServer(API_ORDERS,order);
        alert("Order sent");
    } catch (error) {
        alert("Error: " + error.message);
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
document.getElementById("addRows").addEventListener("click", addRowstoProducts);
document.getElementById("createOrder").addEventListener("submit", createOrder);
loadAllProducts();

