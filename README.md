# MCT2020_Recipes
Dit repository bevat deeloplossingen van MCT labo's.
Het dient vooral als voorbeeldcode. 
De architectuur steunt volledig op herbruikbare en uitbreidbare microservices. 
Ook studenten kunnen/mogen aanpassingen posten. 

# Microservices - eigenschappen
Recipes_DB services (Docker container): 
- RepoPattern (Een onbestaande categorie wordt bij opvoeren ve gerecht automatisch aangemaakt en informeert de user).
- Errorcontroller met hardcoded loggen van errors (Serilog).
- GetCategories informeert de Hub RealTime indien een request kwam (illustreert Hubdependancy)
- Seeder maakt database en content aan op Docker.

Recipes-Unittesten en integratietesten.
- worden niet gebuild in de container (performanter).
- worden opgeroepen in de staging omgeving via Docker-Compose
- Unittesten worden uitgevoerd bij het maken van de Recipes_DB container en moeten succesvol zijn om online te komen.
- Unittesten gebruiken de Recipes_DB seeder als testdata.

CartServices en OrderServices (Beide in eigen Docker container):
- verzorgen samen het bestellen van gerechten voor een ingelogde user.
- OrderService wordt over een bestelling geïnformeerd door de CartService via AMQP
- AMQP wordt verzorgd door RabbitMQ in een Docker container.
- Authenticatie is nodig en gebruikt JWT.
- Seeder maakt een testbestelling aan op een bestaand UserId.

RealtimeServices (Docker Container)
- chatten verloopt via objecten.
- images worden realtime verstuurd als base64.
- frontend website met javascript laat realtime testen toe

Docker-compose verzorgt het beheer van de microservices

# Identity-services
- runnen op een afzonderlijke webserver (=website niet in Docker) en databaseserver(SQL).
- zowel de IdentityUser (<User>)  als IdentityRole (<Role>)zijn customised 
- maken gebruik van de standaard UserManager en RoleManager
- zorgt voor aanmaak van JWT tokens en claims voor doorgeven van informatie naar de microservices.
- Rolemanagement en usermanagement zijn beschikbaar voor de admin via een MVC website met Razor. 
  
# Gateway voor single access point
- runt in Docker container (ocelot) 
- is het single accesspoint naar de andere Docker container services.
- voorziet authenticatie  via een AuthenticationProviderKey



