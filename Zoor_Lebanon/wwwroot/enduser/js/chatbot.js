document.addEventListener("DOMContentLoaded", function () {
    const chatbotButton = document.getElementById("chatbot-button");
    const chatbotModal = document.getElementById("chatbot-modal");
    const chatbotMessages = document.getElementById("chatbot-messages");
    const predefinedQuestions = document.getElementById("predefined-questions");
    const closeButton = document.querySelector(".close-chatbot");

    // Start closed by default
    let isChatbotOpen = false;
    chatbotModal.style.display = "none";
    // Function to toggle chatbot modal visibility
    function toggleChatbot() {
        isChatbotOpen = !isChatbotOpen;
        if (isChatbotOpen) {
            chatbotModal.style.display = "flex"; // Ensure it works with flex styles
        } else {
            chatbotModal.style.display = "none";
        }
    }

    // Open/Close chatbot on button click
    chatbotButton.addEventListener("click", toggleChatbot);

    // Close chatbot when the close button is clicked
    closeButton.addEventListener("click", function () {
        isChatbotOpen = false;
        chatbotModal.style.display = "none";
    });

    // Predefined responses
    const predefinedResponses = {
        "adventure": "We offer great adventure packages! Would you like to explore them?",
        "contact": "You can contact us at info@zoorlebanon.com or call +961-123456.",
        "booking": "You can book directly on our platform. Need help finding something?",
        "prices": "Our packages are competitively priced to offer the best experience. What type of activity are you looking for?",
        "loyalty points": "You can earn loyalty points with every booking! These can be redeemed for discounts or special offers."
    };

    // Handle predefined questions
    predefinedQuestions.addEventListener("click", function (event) {
        if (event.target && event.target.tagName === "BUTTON") {
            const question = event.target.dataset.question;
            handleUserInput(question);
        }
    });

    // Handle user input and bot responses
    function handleUserInput(userInput) {
        chatbotMessages.innerHTML += `<p><strong>You:</strong> ${userInput}</p>`;
        const botResponse = predefinedResponses[userInput] || "I'm sorry, I didn't understand that. Can you try rephrasing?";
        chatbotMessages.innerHTML += `<p><strong>Zoor Bot:</strong> ${botResponse}</p>`;
        chatbotMessages.scrollTop = chatbotMessages.scrollHeight; // Keep scroll at the bottom
    }
});