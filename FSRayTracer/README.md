
# F# Ray Tracer

This project contains all the logic needed to implement the ray tracing approach provided by _The Ray Tracer Challenge_ book.

The exposed API has the following main aspects:

* **Geometries** - A _geometry_ solely provides logic to determine where/if it has been intersected by a ray of light.
  - This logic is provided by an  _intersector_ function which maps an incoming ray into a (potentially empty) list of intersection points in **object** space.
  - With the exception of planes and triangle meshes, all geometries are assumed to be solid in that they have volume.
  
* **Transformations** - A _transformation_ describes a mapping between points in object space and points in world space.
  - As an example, the basic sphere geometry only provides for a unit sphere centered at the world origin.
  - In order to use such a sphere within a scene, it will likely need to be both scaled and translated.

* **Materials** - Provide a mapping (via a _material mapper_) between a point on a geometry in object space and the material to be rendered.
  - They have properties such as _shinyness_ and _relfectiveness_ (for example).

* **Scene Objects** - This is a combination of a _geometry_, _transformation_ and _material_.

* **Scene** - Represents a collection of _scene objects_ that can be rendered.

* **View Settings** - Specifies the location of the 'camera' used to render the scene.

* **Camera Settings** - Specifies the properties of the 'canvas' onto which the scene will be rendered.

* **Light Sources** - Represent point-lights in world space that have both colour and intensity.

* **Colourizer** - Determines the colour of a point in world space based on what is located at that point (if anything), the material at that same location and the available lighting.
 
* **Renderer** - Renders the scene using all of the inputs above.

This is illustrated by the hierarchy below:

[![](https://mermaid.ink/img/eyJjb2RlIjoiZ3JhcGggQlRcbiAgICBHW0dlb21ldHJ5XSAtLT4gVEdbVHJhbnNmb3JtZWQgR2VvbWV0cnldXG4gICAgVFtUcmFuc2Zvcm1hdGlvbl0gLS0-IFRHXG4gICAgTVtNYXRlcmlhbF0gLS0-IFNPMVtTY2VuZSBPYmplY3RdXG4gICAgVEcgLS0-IFNPMVxuICAgIFNPMSAtLT4gU1tTY2VuZV1cblxuICAgIFNPMltTY2VuZSBPYmplY3QuLi5dIC0tPiBTXG4gICAgU08zW1NjZW5lIE9iamVjdC4uLl0gLS0-IFNcbiAgICBTTzRbLi4uXSAtLT4gU1xuXG4gICAgTFNbTGlnaHQgU291cmNlc10gLS0-IENbQ29sb3VyaXplcl0gXG5cbiAgICBTIC0tPiBSXG4gICAgVlNbVmlldyBTZXR0aW5nc10gLS0-IFJbUmVuZGVyZXJdXG4gICAgQ1NbQ2FtZXJhIFNldHRpbmdzXSAtLT4gUlxuICAgIEMgLS0-IFIiLCJtZXJtYWlkIjp7InRoZW1lIjoiZm9yZXN0In0sInVwZGF0ZUVkaXRvciI6ZmFsc2UsImF1dG9TeW5jIjp0cnVlLCJ1cGRhdGVEaWFncmFtIjpmYWxzZX0)](https://mermaid-js.github.io/mermaid-live-editor/edit/##eyJjb2RlIjoiZ3JhcGggQlRcbiAgICBHW0dlb21ldHJ5XSAtLT4gVEdbVHJhbnNmb3JtZWQgR2VvbWV0cnldXG4gICAgVFtUcmFuc2Zvcm1hdGlvbl0gLS0-IFRHXG4gICAgTVtNYXRlcmlhbF0gLS0-IFNPMVtTY2VuZSBPYmplY3RdXG4gICAgVEcgLS0-IFNPMVxuICAgIFNPMSAtLT4gU1tTY2VuZV1cblxuICAgIFNPMltTY2VuZSBPYmplY3QuLi5dIC0tPiBTXG4gICAgU08zW1NjZW5lIE9iamVjdC4uLl0gLS0-IFNcbiAgICBTTzRbLi4uXSAtLT4gU1xuXG4gICAgTFNbTGlnaHQgU291cmNlc10gLS0-IENbQ29sb3VyaXplcl0gXG5cbiAgICBTIC0tPiBSXG4gICAgVlNbVmlldyBTZXR0aW5nc10gLS0-IFJbUmVuZGVyZXJdXG4gICAgQ1NbQ2FtZXJhIFNldHRpbmdzXSAtLT4gUlxuICAgIENbQ29sb3VyaXplcl0gLS0-IFIiLCJtZXJtYWlkIjoie1xuICBcInRoZW1lXCI6IFwiZm9yZXN0XCJcbn0iLCJ1cGRhdGVFZGl0b3IiOmZhbHNlLCJhdXRvU3luYyI6dHJ1ZSwidXBkYXRlRGlhZ3JhbSI6ZmFsc2V9)

Please refer to the [SandBox](https://github.com/Rich-F-G-Mills/RayTracerChallenge/blob/master/SandBox/Program.fs) program for an example of creating and rendering a scene.