from transformers import pipeline
from flask import Flask, request, jsonify

# Load sentiment analysis pipeline
sentiment_analyzer = pipeline("sentiment-analysis")
app = Flask(__name__)

# List of inappropriate words to filter
BAD_WORDS = ["awful", "terrible", "stupid", "hate", "worst", "disgusting", "horrible"]

@app.route("/process_review", methods=["POST"])
def process_review():
    data = request.json
    review_text = data.get("review", "")
    
    # Check for bad words
    if any(bad_word in review_text.lower() for bad_word in BAD_WORDS):
        return jsonify({
            "status": "rejected", 
            "sentiment": "NEGATIVE",
            "reason": "Inappropriate language detected."
        })

    # Sentiment analysis
    sentiment = sentiment_analyzer(review_text)[0]
    sentiment_label = sentiment["label"]
    sentiment_score = sentiment["score"]

    # Debugging: Log the sentiment score
    print(f"Review Text: {review_text}")
    print(f"Sentiment Label: {sentiment_label}, Sentiment Score: {sentiment_score}")

    # Adjusted thresholds for neutral sentiment
    if sentiment_label == "NEGATIVE" and sentiment_score > 0.85:
        return jsonify({
            "status": "rejected", 
            "sentiment": sentiment_label,
            "reason": "Review is overly negative."
        })
    elif sentiment_label == "NEGATIVE" and sentiment_score <= 0.85:
        return jsonify({
            "status": "approved", 
            "sentiment": "NEUTRAL"
        })
    elif sentiment_label == "POSITIVE":
        return jsonify({
            "status": "approved", 
            "sentiment": "POSITIVE"
        })
    else:
        return jsonify({
            "status": "approved", 
            "sentiment": "NEUTRAL"
        })

if __name__ == "__main__":
    app.run(debug=True, host="0.0.0.0", port=5000)
