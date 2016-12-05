About
================
Progressive Delaunay is an algorithm that triangulates a set of points in 2D environments. This algorithm was designed to achieve a lower memory footprint. The result is a table named DCEL which contains all of ready to draw edges.

Memory structures
================
There are 3 important structs to take into account, because you can imagine what `Vector3` and `Vector2` structs are. These structs will allow you to define and move through the current set of triangles. The first one is named `face` and contains all faces (triangles) and a reference to a single edge, which is used to form the triangle. The second one is named `vertex` and contains the coordinates of every point and a reference to an edge which passes through. The last one, called `DCEL`, contains all the edges as the references of the faces of both sides and bounding vertices. 

You can see a representation in the following figure.

![alt tag](https://github.com/JVanDamme/ProgressiveDelaunay/blob/master/img/DCEL.jpg)
