"use strict";

const API_PRODUCTS = "https://localhost:7001/api/products";
const API_ORDERS = "https://localhost:7001/api/orders";
const messageElement = document.getElementById("message");
function showMessage(message, isError = false) {
    messageElement.textContent = message;
    messageElement.className = isError ? "Error" : "sucess";
}