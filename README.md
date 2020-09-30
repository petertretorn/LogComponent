# LogComponent

Unit tests covering the stated use cases have been written

A log sink has been factored out to a seperate component

Current time has been factored out as an external dependency to facilitate testing

Thorough refactoring of AsyncLog class

_______________________________________

The component can alternatively be implemented using Akka.NET and actors.
A log receiver actor recieves log messeges from client application and stores them in internal state.
The log receiver periodically messeges a log writer actor that writes messeges to a specified sink.
The receiver actor functions as a state machine withe 3 states: writing, flushing and closed and state 
transitioning is triggered by client requesting to stop with/without flushing etc.

Benefits of using actors include no risk of race condition accessing the received logrequests between receiver actor and writer actor,
plus scalability, the actor that is bottleneck can be scaled
