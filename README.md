# The Ray Tracer Challenge

![](https://pragprog.com/titles/jbtracer/the-ray-tracer-challenge/jbtracer_hu6d5b8b63a4954cb696e89b39f929331b_958829_250x0_resize_q75_box.jpg)

For a long while, I had this [book](https://pragprog.com/titles/jbtracer/the-ray-tracer-challenge/) by Jamis Buck on my bookshelf but had never actually taken the time to work through it.

As someone who constantly feels like they need to prove their programming prowess (purely self-inflicted), I felt the time was right to take the plunge and work through it.

Although the book does give an indication of the structure to use for this project, I set up the project using a layout that I felt was best and that lended itself to the programming language I was intending to use.

Having recently discovered the joys of F#, I felt that I wanted to try a functional approach to implementing the logic.

It was incredibly satisfying being able to leverage techniques such as:
* Functional composition; where more complex operations are built by combining simpler ones together.
* Use of (partially applied) functions instead of interfaces.
* Pattern matching using active patterns.
* Immutability; the entire rendering pipeline is [_pure_](https://en.wikipedia.org/wiki/Pure_function) in that there are no side-effects or changes in mutable state.

Within this repository, you'll see the following items:

* **FSRayTracer** - This is the [F#](https://fsharp.org/) library I wrote that implements the ray-tracing algorithm provided by the book.
* **SandBox** - This is an executable that links to the above library in order to render a sample scene using the exposed API.
* **TestPack** - Runs the various tests that are shown in (or inspired by) the book using the [xUnit](https://xunit.net/) testing framework.

In terms of **future developments**:
* In hindsight, I wish I had using the [FsCheck](https://github.com/fscheck/FsCheck) interop with xUnit when writing the tests.
  - The resulting code would have been more idiomatic to F#.
* Use of benchmarking to check for more performant approaches.
* Support for refractions.
* Use of parallelism within the rendering pipeline.

