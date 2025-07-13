import math
from svgpathtools import svg2paths

def extract_polygon_points(svg_file):
    paths, _ = svg2paths(svg_file)
    path = max(paths, key=lambda p: p.length())
    points = []
    for e in path:
        points.append((e.start.real, e.start.imag))
    unique_points = []
    for pt in points:
        if pt not in unique_points:
            unique_points.append(pt)
    return unique_points

def centroid(points):
    x = sum(p[0] for p in points) / len(points)
    y = sum(p[1] for p in points) / len(points)
    return (x, y)

def compare_polygons(polyA, polyB):
    # polyA: inner polygon, polyB: outer polygon (should have more points)
    cA = centroid(polyA)
    cB = centroid(polyB)
    print(f"\nComparing {len(polyA)}-point polygon to {len(polyB)}-point polygon")
    print("Original vertices movement:")
    for i, (pA, pB) in enumerate(zip(polyA, polyB[:len(polyA)])):
        dx = pB[0] - pA[0]
        dy = pB[1] - pA[1]
        dist = math.hypot(dx, dy)
        angleA = math.atan2(pA[1] - cA[1], pA[0] - cA[0])
        angleB = math.atan2(pB[1] - cB[1], pB[0] - cB[0])
        angle_diff = math.degrees(angleB - angleA)
        print(f"  Vertex {i}: Δx={dx:.2f}, Δy={dy:.2f}, Δr={dist:.2f}, Δθ={angle_diff:.2f}°")
    print("\nNew vertices in outer polygon:")
    for i in range(len(polyA), len(polyB)):
        x, y = polyB[i]
        angle = math.degrees(math.atan2(y - cB[1], x - cB[0]))
        dist = math.hypot(x - cB[0], y - cB[1])
        print(f"  New vertex {i}: r={dist:.2f}, θ={angle:.2f}°")

def angle_from_center(center, pt):
    return math.atan2(pt[1] - center[1], pt[0] - center[0])

def best_rotation(polyA, polyB):
    # Use only the first len(polyA) points of polyB
    cA = centroid(polyA)
    cB = centroid(polyB)
    anglesA = [angle_from_center(cA, pt) for pt in polyA]
    anglesB = [angle_from_center(cB, pt) for pt in polyB[:len(polyA)]]
    diffs = [(b - a + math.pi) % (2 * math.pi) - math.pi for a, b in zip(anglesA, anglesB)]
    avg_diff = sum(diffs) / len(diffs)
    return math.degrees(avg_diff)

# Extract polygons from your three SVGs
poly1 = extract_polygon_points("1.svg")
poly2 = extract_polygon_points("2.svg")
poly3 = extract_polygon_points("3.svg")

print(f"poly1: {len(poly1)} points")
print(f"poly2: {len(poly2)} points")
print(f"poly3: {len(poly3)} points")

compare_polygons(poly1, poly2)
compare_polygons(poly2, poly3)

rot12 = best_rotation(poly1, poly2)
rot23 = best_rotation(poly2, poly3)

print(f"Estimated rotation from layer 1 to 2: {rot12:.2f} degrees")
print(f"Estimated rotation from layer 2 to 3: {rot23:.2f} degrees")