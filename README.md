# MindBubble
## Concept
Our concept introduces "Foxy," a friendly virtual companion designed to assist users with location-specific information using voice commands. Imagine a participant in next year’s hackathon asking Foxy: "Where did participants from last year meet before the start of the hackathon?"

Our service processes this request by running on Azure, leveraging an Azure Function for scalable, on-demand execution. The function integrates with OpenAI ChatGPT to search and analyze data from various social media platforms and review sites, focusing on posts with relevant hashtags from the previous year's event. It utilizes the user's current GPS location to narrow down the search. The gathered information is aggregated, categorized, and grouped into meaningful insights before being sent back to the user's device.

With the precise GPS and VPS (Visual Positioning System) capabilities of the device, the results are displayed as interactive AR bubbles. General posts appear floating around the user, while specific locations, such as restaurants, bars, or lobby areas mentioned in last year’s posts, are highlighted in their real-world or virtual equivalents.

This seamless integration of Azure's serverless architecture, OpenAI's powerful language model, and advanced mixed reality technologies offers an engaging and intuitive way for users to explore and interact with historical social media data relevant to their surroundings.

### System diagram
<p align="center"><img src="/Images/MindBubble-vision.png" alt="system diagram vision" width="800"></p>
