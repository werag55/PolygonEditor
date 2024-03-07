# PolygonEditor

<img width="1280" alt="image" src="https://github.com/werag55/PolygonEditor/assets/147431062/7f291c15-7ab5-4304-b7ac-419bb2a08bb1">

## Menu bar:
1) Mode: possible to change mode between drawing and editing.
2) Settings: possible to change settings, i.e. the line drawing algorithm (library/Bresenham), background color, offset algorithm.

## Drawing mode:
1) LMB (Left Mouse Button): placing a vertex at the selected point or, if a point is selected near the starting point of the vertex, ending the polygon (provided that it already has at least three vertices)
2) RMB: ending the polygon (provided that it already has at least three vertices - otherwise abandoning the part of the polygon drawn so far)
3) Color button: change the current drawing color, i.e. each edge, including the currently drawn one, will be drawn with the selected color.
4) ESC: abandoning the previously drawn part of the polygon

## Editing mode:
1) LMB: if you click on a point near the vertex/near the edge/inside the polygon, start moving the vertex/edge/polygon respectively.
2) Double click: If you click a point near an edge, add a vertex on that edge at the point closest to the cursor location
3) RMB: when clicking on a point
- near a vertex, remove that vertex (provided the polygon has more than three vertices)
- near the edge, displaying a context menu with the option of adding a vertical or horizontal edge relationship
- inside a polygon, determining the offset of this polygon in accordance with the current value on the track bar control (until we press somewhere else, this offset can be changed smoothly)
5) ESC: in case the polygon was previously selected with RMB (and was not deselected by clicking somewhere else), deleting this polygon

## Relationship algorithm

For each polygon I store a list of edges (List<List<Edge> edges in the Manager class). In the Edge class, I have a field that informs what, if any, relationship is imposed on a given edge (the direction, Any field means that no 
relationship is imposed on the edge).

After right-clicking on an edge in Editing mode, a context menu appears, through which we can select a relationship for a given edge (before displaying the menu, I check whether the neighboring edges already have a relationship imposed, 
if so, I set the field corresponding to a given relationship as unavailable in the context menu so that it is impossible was to add the same relations to adjacent sides). The Context menu allows you to delete a relationship in the same 
way by deselecting the previously selected one. Any changes to a selected edge are saved in the corresponding Edge object.

After adding the relationship (and also when moving the vertex/edge), I call the updateRel(int i, int j) function from the EditMode class, which updates the coordinates for the j-th edge of the i-th polygon in the program if necessary, 
i.e. if a relationship exists. the vertex next after the jth and preceding the j-th, so that the relations are preserved (i.e. equalizing the X coordinate in the case of the Vertical relation and the Y coordinate in the case of the 
Horizontal relation of the neighboring vertex to the coordinate of the j-th vertex).

When removing/adding a vertex, I remove relations from adjacent edges.

When drawing a scene, I check whether a given edge has any restrictions, if so, I designate a section parallel to it, and then the middle point of this section, in order to draw an icon related to the relationship at a given distance 
from the edge - the letter H for horizontal edges and the letter V for vertical edges.

## Offset determination algorithm
I have prepared several options
2) findOffsetPolygon function in the EditMode class

For each vertex, the algorithm determines segments parallel to the edges of which this vertex is a part, finds the intersection of these segments (their extensions) and thus determines the shifted vertex, which is the vertex of the offset 
polygon. The points that make up the offset polygon are passed through the points output parameter and saved for the appropriate polygon in the notFixed list in the Manager class.

1) FindOffsetPolygon function in the EditMode class

As in 2. but I also remove self-intersections in the offset polygon - fixSelfIntersections function in the EditMode class.

Let points - "unrepaired" points of the offset polygon, offset - target, repaired set of points of the offset polygon.

Let's assume that the polygon was drawn counterclockwise (wp. take the value opposite to the given offset value).
Starting from i=0, we add the i-th vertices with points to the offset until we encounter a self-intersection, i.e. the intersection of the i-th edge (remember its index in removeStart) with one of the next ones (apart from the neighboring 
one, of course), let's call its addStart index. We check intersections with subsequent edges from the last edge. After hitting a self-intersection, we add the intersection point to the offset, and until we find the "exit" from the polygon, 
i.e. another self-intersection, we do not add the considered vertices to the offset. When looking for an "exit", you should not only look for intersections with subsequent edges, but also check whether the currently considered edge does not 
intersect with the removeStart edge, i.e. whether the entry and exit from the interior of the polygon occurred at the same point (loop). In case we found an exit through one of the next edges, let's call it addEnd, we add copies of all 
vertices between addStart and addEnd to offset, and then add the output intersection point to offset. For loops, you just need to add a starting point. This removes the space where the polygon overlaps itself.

The points that make up the offset polygon are saved for the appropriate polygon in the offsets list in the Manager class.

3) DrawOffsetPolygon function in the EditMode class

An algorithm that draws an offset polygon in the following way: for each vertex on the edge that starts with this vertex, it paints a rectangle with the other side of the offset length (by determining a segment parallel to the edge, 
moved by the offset) and paints a circle with the offset radius and center at the vertex . The offset values for the corresponding polygons are saved in the offsetsValue list in the Manager class.

For all algorithms 1.-3., in the case of polygons drawn clockwise, the offset sign should be changed to negative because wpp. it would be drawn in the center of the polygon.

When editing/moving a polygon, its offset is determined again, the same when modifying the offset value on the track bar control for the selected RMB in the Editing polygon mode.
