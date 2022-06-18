As suggested in the task description, I have added some test coverage and aimed to refactor the code only if needed for adding the coverage or improving reliability.

Test coverage may not be extensive, error handling may still be missing in many places, yet considering the state of the project, with the 3 hours time constraint it is not achievable (I have exceeded it anyway - adding any reasonable coverage simply required more time).

## Changes done to the .NET service:
- Injecting HttpClientFactory
- Added some model/response validation
- Added URL param encoding (to avoid URL injection)
- Added some simple response error handling
- Added some basic tests

## What else could be done there to make the code cleaner / more robust:
- MVC controller could be replaced with WebAPI if it's not meant to serve razor/blazor templates
- Business logic could be moved from controller to some provider (would make testing easier in the long term)
- Error handling could be more advanced (e.g. JSON deserialization errors could return error responses based on the excepton kind; right now it will throw exception resulting in a generic 50x error)
- Default exception handling could be configured through an attribute
- The service URL could be moved to config to allow changing it without the necessity of redeploying the service
- Logging/diagnostics could be added
- ... 

## Changes done in the client app
- extracted the data fetch logic into a separate file
- adjusted the logic in the way in which the state is only updated once every 5s, not for every product
- changed setInterval to setTimeout and changed logic to prevent excessive rerendering and leaks
- Added some test coverage

## What else could be done there, if only there would be more time:
- Test coverage could be added to the logic in temperature.js (would require mocking the global fetch function)
- error handling could be adjusted (now the data will be empty or just stop being refreshed if something fails)
- Tests should also account for the cases of errors (how should the app behave when there's an error)
- The logic whether the temperature is too high or too low could be moved outside of the view to a function (for single responsibility)
- typescript typings could be leveraged
- error boundaries could be defined
- interval could be moved to config
- the rendering loop using useEffect and useState if fragile (e.g. re-rendering the component for a different reason would reset the timer); it would be better to build a custom hook which would subscribe to the service updates e.g. through a websocket (of course that would require the service to implement SignalR hub for example)  