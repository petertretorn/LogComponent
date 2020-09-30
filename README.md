# LogComponent

Unit tests covering the stated use cases have been written

A log sink has been factored out to a seperate component

Current time has been factored out as an external dependency to facilitate testing

Thorough refactoring of AsyncLog class

---

The component can alternatively be implemented using Akka.NET and actors.
A log receiver actor receives log messeges from client application and stores them in internal state.
The log receiver periodically messeges a log writer actor that writes messeges to a specified sink.
The receiver actor functions as a state machine with 3 distinct states, writing, flushing and closed.

State transitions:

* writing -> flushing:  triggered by client sending stopWithFlush message, 
* flushing -> closed: triggered when internal buffer is empty
* writing -> closed: triggered by client sending stopWithNoFlush message, 

Benefits of using actors include a cleaner architecture, avoidance of race conditions and scalability.
