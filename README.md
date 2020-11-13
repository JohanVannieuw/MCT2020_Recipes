# MCT2020_Recipes
Dit repository bevat deeloplossingen van MCT labo's.
Het dient vooral als voorbeeldcode. 
De architectuur steunt volledig op herbruikbare en uitbreidbare microservices. 
Ook studenten kunnen/mogen aanpassingen posten. 

# Microservices - eigenschappen
Recipes_DB services (Docker container): 
- RepoPattern (onbestaande categorie bij opvoeren ve gerecht wordt auto-aangemaakt
- Errorcontroller.
- GetCategories informeert de Hub (RealTime) over bevraging.
- Seeder maakt database en content aan op Docker.

Recipes-Unittesten en integratietesten.
- worden niet gebuild in de container (performanter).
- unittesten worden uitgevoerd bij het maken van de Recipes_DB container en moeten succesvol zijn.

CartServices en OrderServices (Beide in eigen Docker container):
- bestellen van gerechten.
- OrderService wordt geïnformeerd via AMQP (RabbitMQ).
- authenticatie is nodig en gebruikt JWT.
- Seeder maakt bestelling aan op bestaand UserId

RealtimeServices (Docker Container)
- chatten verloopt via objecten.
- images worden realtime verstuurd als base64.

Docker-compose verzorgt het beheer van de microservices

# Identity-services
- runnen op een afzonderlijke webserver (=website niet in Docker) en databaseserver(SQL).
- zowel de IdentityUser (<User>)  als IdentityRole (<Role>)zijn customised 
- zorgt voor aanmaak van JWT tokens en claims voor doorgeven van informatie naar de microservices.
- rolemanagement en usermanagement beschikbaar voor de admin via een MVC website met Razor. 
  
# Gateway voor single access point
- runt in Docker container (ocelot) 
- single accesspoint naar de andere Docker container services.
- authenticatie  via een AuthenticationProviderKey



