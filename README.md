# Projekt_Avancerad.NET - Reasoning  
In the class library project, I only put the models with a folder for DTOs which I use for omitting the properties that I don't want the client to fill when adding or changing an object via requests to the API. 
I also implemented a mapper to transfer properties between the DTOs and the actual models. However, EF doesn't track objects that are being modified by the mapper, so I had to assign properties manually when dealing with [HttpPut]. 

Due to the limited predefined types of the objects that the API's controller needs to handle,  I've figured that it would be convenient if I only use a single repository (class & interface) for handling all the objects. That resulted in very less code and fewer files. So it basically became as follows:
One DbContext => One repository => One controller => One API.
It's worth mentioning that if the class library and the API are going to need to expand a lot and return many other objects, then it would probably be better to reorganize the entire architecture and do some separation for different objects in the controllers and repositories. 

I also made sure the Linq queries are as efficient as possible to have a much cleaner and readable code which only gives one single query(job) for each method in the repo-class. That has given the result-methods in the controller a very clear view when calling the repository methods.
The result-methods are well equipped to check and handle errors to make the API pretty responsive in all possible cases and results. 
