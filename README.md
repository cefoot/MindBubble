
<p align="center"><img src="/Images/Logo_white.png" alt="system diagram vision" width="800"></p>

# MindBubble

## Concept

Our concept introduces "Foxy," a friendly virtual companion designed to assist users with location-specific information using voice commands. Imagine a participant in next year’s hackathon asking Foxy: "Where did participants from last year meet before the start of the hackathon?"

Our service processes this request by running on Azure, leveraging an Azure Function for scalable, on-demand execution. The function integrates with OpenAI ChatGPT to search and analyze data from various social media platforms and review sites, focusing on posts with relevant hashtags from the previous year's event. It utilizes the user's current GPS location to narrow down the search. The gathered information is aggregated, categorized, and grouped into meaningful insights before being sent back to the user's device.

With the precise GPS and VPS (Visual Positioning System) capabilities of the device, the results are displayed as interactive AR bubbles. General posts appear floating around the user, while specific locations, such as restaurants, bars, or lobby areas mentioned in last year’s posts, are highlighted in their real-world or virtual equivalents.

This seamless integration of Azure's serverless architecture, OpenAI's powerful language model, and advanced mixed reality technologies offers an engaging and intuitive way for users to explore and interact with historical social media data relevant to their surroundings.

### System diagram

<p align="center"><img src="/Images/MindBubble-vision.png" alt="system diagram vision" width="800"></p>

## current state

Connecting to real social media platforms proved to be a significant challenge. Some platforms require a subscription to their API, while others mandate being a verified company. After extensive testing with multiple platforms, we decided to simplify the process by leveraging another powerful tool: ChatGPT.

Our Azure Function now makes an initial call to ChatGPT to generate simulated social media posts based on the provided keyword and GPS coordinates. These generated posts are then grouped, categorized, and sent back to the client for display.

We currently have two fully functional clients:

### Meta Quest 3 (Mixed Reality App)

This app uses passthrough mode to create a mixed reality experience. When the user interacts with Foxy, the app first retrieves rough GPS coordinates using the public IP address. The service is then queried with the keyword and these approximate coordinates. The resulting categories of posts are displayed in front of the user as interactive bubbles. When a category is popped, detailed posts are revealed in additional bubbles, creating an engaging and immersive experience.

### Phone AR App

On the phone, precise positioning is achieved using Google Geospatial VPS. Foxy is virtually placed next to the user on the ground, serving as a friendly companion. When a keyword is entered, the app queries the service with both the keyword and precise coordinates. The results are then displayed in front of the user, similar to the Quest app, with categories and detailed posts rendered as interactive AR bubbles.

This streamlined solution combines the power of Azure, ChatGPT, and cutting-edge AR technologies to deliver an intuitive and dynamic experience across devices. Despite the challenges, we have successfully built a system that bridges the gap between virtual content creation and interactive AR experiences.





<p align="center"><img src="/Headset-VR/Assets/Images/mindBubble1.png" alt="system diagram vision" width="800"></p>
