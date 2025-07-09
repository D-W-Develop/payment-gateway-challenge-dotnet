# Scope assumptions

I have assumed that various things which would be significant concerns for a real application are out of scope for this exercise, particularly:
* Authorization
* Observability 
* Configuring HttpClient for resilience with retries etc.
* Deploying the infrastructure
* Deploying the code
* Persistent data storage
* End to end tests with non-mocked acquring bank APIs

# Business logic assumptions

In some places the requirements are ambiguous. In 'real life' I'd want to have a chat with a product manager; 
for this exercise I've made some assumptions:
* Where the acquiring bank fails to handle the request I've assumed it's reasonable to return HTTP status BadGateway and payment status rejected
* I've assumed that we only want to persist the details of payments which succeeded (the spec says "allow a merchant to retrieve details of a **previously made payment**). YMMV. 
* I've assumed that if your card says expiry 01/2025 you can use it up to the end of 31 Jan 2025
 **UTC** (which may be the early hours of 1 Feb 2025 where the merchant is). I have no idea what the rules are in real life. 

 # Other design considerations

 I've designed my approach to this exercise with the same considerations in mind:
 * Modularity; each class should have a single responsibility
 * Readability; descriptive names, simple methods
 * Explicit handling for foreseeable failure modes
 * High (but proportionate to risk and complexity) test coverage