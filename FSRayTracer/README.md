
# FS Ray Tracer

This project contains all the logic needed to implement the ray tracing approach provided by _The Ray Tracer Challenge_ book.

The exposed API has the following main aspects:

* **Geometries** - A _geometry_ solely provides logic to determine where/if it has been intersected by a ray of light.
  - This logic is provided by an  _intersector_ function which maps an incoming ray into a (potentially empty) list of intersection points in **object** space.
  - With the exception of planes and triangle meshes, all geometries are assumed to be solid in that they have volume.
  
* **Transformations** - A _transformation_ describes a mapping between points in object space and points in world space.
  - As an example, the basic sphere geometry only provides for a unit sphere centered at the world origin.
  - In order to use such a sphere within a scene, it will likely need to be both scaled and translated.

* **Materials** - Provide a mapping (via a _material mapper_) between a point on a geometry in object space and a colour to be rendered.
  - This mapping depends on characteristics of the material itself (eg. shininess) and light sources within the scene.

* **Scene Objects** - This is a combination of a _geometry_, _transformation_ and _material_.

* **Scene** - Represents a collection of _scene objects_ that can be rendered.

This is illustrated by the hierarchy below:

[![](https://mermaid.ink/img/eyJjb2RlIjoiZ3JhcGggQlRcbiAgICBHW0dlb21ldHJ5XSAtLT4gVEdbVHJhbnNmb3JtZWQgR2VvbWV0cnldXG4gICAgVFtUcmFuc2Zvcm1hdGlvbl0gLS0-IFRHXG4gICAgTVtNYXRlcmlhbF0gLS0-IFNPMVtTY2VuZSBPYmplY3RdXG4gICAgVEcgLS0-IFNPMVxuICAgIFNPMSAtLT4gU1tTY2VuZV1cblxuICAgIFNPMltTY2VuZSBPYmplY3QuLi5dIC0tPiBTXG4gICAgU08zW1NjZW5lIE9iamVjdC4uLl0gLS0-IFNcbiAgICBTTzRbLi4uXSAtLT4gU1xuIiwibWVybWFpZCI6eyJ0aGVtZSI6ImRlZmF1bHQifSwidXBkYXRlRWRpdG9yIjpmYWxzZSwiYXV0b1N5bmMiOnRydWUsInVwZGF0ZURpYWdyYW0iOmZhbHNlfQ)](https://mermaid-js.github.io/mermaid-live-editor/edit##eyJjb2RlIjoiZ3JhcGggQlRcbiAgICBHW0dlb21ldHJ5XSAtLT4gVEdbVHJhbnNmb3JtZWQgR2VvbWV0cnldXG4gICAgVFtUcmFuc2Zvcm1hdGlvbl0gLS0-IFRHXG4gICAgTVtNYXRlcmlhbF0gLS0-IFNPMVtTY2VuZSBPYmplY3RdXG4gICAgVEcgLS0-IFNPMVxuICAgIFNPMSAtLT4gU1tTY2VuZV1cblxuICAgIFNPMltTY2VuZSBPYmplY3QuLi5dIC0tPiBTXG4gICAgU08zW1NjZW5lIE9iamVjdC4uLl0gLS0-IFNcbiAgICBTTzRbU2NlbmUgT2JqZWN0Li4uXSAtLT4gU1xuIiwibWVybWFpZCI6IntcbiAgXCJ0aGVtZVwiOiBcImRlZmF1bHRcIlxufSIsInVwZGF0ZUVkaXRvciI6ZmFsc2UsImF1dG9TeW5jIjp0cnVlLCJ1cGRhdGVEaWFncmFtIjpmYWxzZX0)

Please refer to the [SandBox](SandBox/Program.fs) program for an example.