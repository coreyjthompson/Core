#**NOT CURRENT - TO BE REVISED**
The architecture has undergone some significant changes and the below description does not currently reflect them.
*This message will be removed once this has been updated.*











# Design Guidelines
[Microsoft Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)

**All** code will follow the above linked Microsoft Design Guidelines unless there are team-agreed alterations to it which must be documented here.

# About MEI Core
MEI **Core** will be at the center of all applications within the MEI Enterprise and will contain all of the code needed to represent the entire problem space, minus specializations specific to clients, of MEI. Each client will have its own projects to contain those specializations. The Core will define the domain model which describes the problem space that all code for all clients will use and/or extend. It will define interfaces for repositories which will be used to perform operations on data and can use domain models as input and output. It will contain client agnostic code that can be used by any client for common operations such as logging, communication, and security. It is called the Core because, as you will see below in the architecture description, it is code that will be the most stable and have the least velocity in regards to change and will be what all other pieces of the system depend on.

## Architecture
![MEI Enterprise Application Architecture](https://meintl0.sharepoint.com/sites/MEICore/Shared%20Documents/General/MEI%20Enterprise%20Application%20Architecture.png)

- In traditional, layered architecture layer dependencies are represented in a top-down manner where a given layer references the layer directly beneath it but not the reverse. In doing that, it places the infrastructure components at the bottom. This causes all other layers, whether directly or transitevly, to be dependent and coupled to that bottom layer. So, if there are changes to the infrastructure then it directly requires the layers above and dependent on it to change as well.
- So instead, this system will use what is termed as the Onion architecture with the intent and advantage of breaking that hard coupling of the traditional, layered architecture and insted make a system that can make changes focused, contained, and not cascading.
- Imagine this architecture as an onion with several layers, one on top and surrounding the other, with a very central layer that all of the other layers surround as opposed to the traditional layered architecture which is top-down in nature.
- Any outer layer can reference any inner layer even if that referenced layer is not directly below the referencing layer.
- Any layer can reference anything within its own layer.
- No layer can reference any outer layer at all.
- What will these layers consist of?
	- At the very center of the onion are the Core's domain models. That domain layer project will have zero references to any other layer in the onion. It can still theoretically have references to external assemblies that are not a part of the onion if need be. But, references from this project should be kept to a very small minimum.
	- On the next layer out, are the client specific domain models.
	- On the next layer out, are Core functions like Logging, Communication, Security. Also, there are interfaces for the repositories and services.
	- On the next and final layer out, is the infrastructure itself. That will be the implementations of the repositories and services that are client specific. Also, the data projects will be here which do the actual acess to the data wherever it resides, database, text files, exteral API's or whatever else. Also on this layer will be the applications themselves, whether that be websites or consoles or whatever as well as their associated test projects.
- Since the infrastructure components are placed on the outside of the onion, it allows them to change implementation without the direct need for change by the inner layers.
- You may notice for example, that the repositories are in a layer that is inside the data layer and therefore is not allowed to reference that data layer since it is outside of its own layer. So then, how does a repository go about getting data from the database. Well, it will techincally still use that data project. But, it will use the concept of Inversion of Control to get an indirect reference to it. That is, the repository project will have interfaces for the repositories and then the infrastructure project will have implementations of those repository interfaces and also the infrastructure project will have a referene to the data project. To use this in practice, we need something like an IoC library which will be used to wire up implementations to interfaces and then allow us to dependency inject them into our classes that need a reference to them.
- Some documention on the Onion Architecture. [Onion Architecture by Jeffrey Palermo](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)

### Domain Models
The domain models of the Core will be all of the classes that represent the entire problem space of MEI. Them and their relationships to each other should prefer to normalized in an object oriented sense isntead of a relational database sense. **Only** the data project should know what our database looks like. All of the other code should see the data in the form of the domain models.
- **Shall** be simple POCO model classes that only inherit from other domain models if needed.
- **Prefer** to extend the Core's domain models through client specific domain models with fields from new requirements rather than modifying them.

### Repositories
The repositories of the Core will be only the interfaces describing them. The infrastructure projects specific to clients will provide the implementations for them. These repositories should provide fairly granular actions on the data in the database.
- **Shall not** return any entity model data types from the data project that isspecific to how the data project gets its data (such as Linq-to-Sql .dbml created models, Entity Framework .edmx created models, etc.).
- **Prefer** to use a mapping mechanism to convert from the data project data type to an appropriate domain model data type if needed.
- **Can** return any data types as necessary including any of the domain models.
- **Prefer** to use direct data access (including ORMs) instead of stored procedures. Use of existing stored procedures if needed is ok until the point that they can be replaced with direct access or if there is an overwhelmingly need to use a stored procedure because of optimizations that are unable to be performed otherwise.
- **Prefer** not to have an explosion in the number of repositories by representing every database table as one. Instead use common groupings of tables in one repository.

### Services
The services of the Core will be only the interfaces describing them. The infrastructure projects specific to clients will provide the implementations for them. These services should provide almost the entirety of the business rules that make up our business. They should use as many repositories and/or any other dependencies that they need to accomplish their tasks. These services will be the entry point into our Core system and what all referencing application will use to interact with it. These services should provide more general actions that encompass larger pieces of functionality than the repositories.
- **Shall not** directly access the database. Use repositories for data access instead.
- **Can** use as many repositories as necessary to accomplish its needs.
- **Can** use any other external dependency (third-party web API, send email, manipulate files, etc.) as necessary to accomplish its task.

### Data
The data project will actually not be part of the Core itself but will instead be part of each client's infrastructure project. It will contain all of the code necessary to interact with the database using whichever data mechanism is appropriate for that client and situation.
- **Can** use any ORM such as Entity Framework to access data.

## Setup Tips for myMEI website
1. 
