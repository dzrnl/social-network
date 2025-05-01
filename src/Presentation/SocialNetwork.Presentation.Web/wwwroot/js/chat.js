const userId = window.currentUserId;
const userFullName = window.currentUserFullName;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveMessage", function (senderId, senderName, message) {
    const container = document.getElementById("chatBox");

    const wrapper = document.createElement("div");
    wrapper.className = "chat-message-wrapper";

    const msg = document.createElement("div");
    msg.textContent = `${senderName}: ${message}`;
    msg.className = "chat-message";

    if (senderId === userId) {
        wrapper.classList.add("own-message");
    }

    wrapper.appendChild(msg);
    container.appendChild(wrapper);
    container.scrollTop = container.scrollHeight;
});

connection.start().catch(err => console.error(err.toString()));

document.getElementById("sendButton").addEventListener("click", function (event) {
    const input = document.getElementById("messageInput");
    const message = input.value.trim();
    if (message) {
        connection.invoke("SendMessage", userId, userFullName, message)
            .catch(err => console.error(err.toString()));
        input.value = "";
    }
    event.preventDefault();
});
