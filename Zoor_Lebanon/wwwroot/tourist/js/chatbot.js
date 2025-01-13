document.addEventListener("DOMContentLoaded", function () {
    // DOM Elements
    const chatbotButton = document.getElementById("chatbot-button");
    const chatbotModal = document.getElementById("chatbot-modal");
    const chatbotMessages = document.getElementById("chatbot-messages");
    const closeButton = document.querySelector(".close-chatbot");
    const predefinedQuestions = document.getElementById("predefined-questions");

    // Open/Close Chatbot
    chatbotButton.addEventListener("click", function () {
        chatbotModal.style.display = chatbotModal.style.display === "block" ? "none" : "block";
    });

    closeButton.addEventListener("click", function () {
        chatbotModal.style.display = "none";
    });

    // Predefined Chatbot Interaction
    const predefinedResponses = {
        "hello": "Hello! How can I assist you today?",
        "adventure": "We offer great adventure packages! Would you like to explore them?",
        "booking": "You can book directly on our platform. Need help finding something?",
        "contact": "You can contact us at info@zoorlebanon.com or call +961-123456.",
        "prices": "Our packages are competitively priced to offer the best experience. What type of activity are you looking for?",
        "loyalty points": "You can earn loyalty points with every booking! These can be redeemed for discounts or special offers."
    };

    // Handle Clickable Predefined Questions
    predefinedQuestions.addEventListener("click", function (event) {
        if (event.target && event.target.tagName === "BUTTON") {
            const userInput = event.target.dataset.question;

            // Add user question to chat
            chatbotMessages.innerHTML += `<p><strong>You:</strong> ${userInput}</p>`;

            // Get bot response
            const botResponse = predefinedResponses[userInput] || "I'm sorry, I didn't understand that. Can you try rephrasing?";

            // Add bot response to chat
            chatbotMessages.innerHTML += `<p><strong>Zoor Bot:</strong> ${botResponse}</p>`;
            chatbotMessages.scrollTop = chatbotMessages.scrollHeight; // Scroll to the bottom
        }
    });
});
