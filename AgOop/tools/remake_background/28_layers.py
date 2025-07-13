import math
import svgwrite

# --- CONFIGURATION ---

width, height = 800, 557
center = (width / 2, height / 2)
initial_radius = 60
initial_sides = 10  # e.g., decagon
num_layers = 28
radius_increment = 15
rotation_deg = 30

# --- COLOR PALETTE (repeat as needed) ---
colors = [
    "#ff9900", "#ffcc33", "#e6f97a", "#b6e7b6", "#a3d3e3", "#7faee3", "#abefb7",
    "#a1ded0", "#b0f5a8", "#9cd3dc", "#b5f999", "#97c7e5", "#bafc8a", "#91b7ef",
    "#bffc7a", "#c4fb6b", "#8ca8f5", "#caf759", "#8799f9", "#cff24a", "#d4ea3c",
    "#d9e22f", "#ded724", "#e3cc19", "#e8bf11", "#edb109", "#f2a204", "#f79300"
]
opacities = [0.9, 0.7, 0.7, 0.8, 0.9, 1.0] * 5  # repeat to cover all layers

# --- INITIAL POLYGON (regular decagon) ---
def regular_polygon(cx, cy, r, n, angle_offset=0):
    return [
        (
            cx + r * math.cos(2 * math.pi * i / n + angle_offset),
            cy + r * math.sin(2 * math.pi * i / n + angle_offset) * (height / width)
        )
        for i in range(n)
    ]

vertices = regular_polygon(center[0], center[1], initial_radius, initial_sides)

# --- GROW POLYGON FUNCTION ---
def grow_polygon(vertices, radius_increment, rotation_deg):
    n = len(vertices)
    new_vertices = []
    angle_offset = math.radians(rotation_deg)
    cx = sum(x for x, y in vertices) / n
    cy = sum(y for x, y in vertices) / n

    for i in range(n):
        # Current vertex
        x1, y1 = vertices[i]
        # Next vertex (wrap around)
        x2, y2 = vertices[(i + 1) % n]
        # Add current vertex
        new_vertices.append((x1, y1))
        # Compute midpoint
        mx, my = (x1 + x2) / 2, (y1 + y2) / 2
        # Vector from center to midpoint
        vx, vy = mx - cx, my - cy
        # Increase length (grow outward)
        length = math.hypot(vx, vy)
        scale = (length + radius_increment) / length
        gx, gy = cx + vx * scale, cy + vy * scale
        # Rotate around center
        dx, dy = gx - cx, gy - cy
        angle = math.atan2(dy, dx) + angle_offset
        r = math.hypot(dx, dy)
        rx, ry = cx + r * math.cos(angle), cy + r * math.sin(angle)
        # Add new vertex
        new_vertices.append((rx, ry))
    return new_vertices

# --- SVG DRAWING ---
dwg = svgwrite.Drawing("28_layers.svg", size=(width, height))
layer_vertices = [vertices]

for i in range(num_layers):
    color = colors[i % len(colors)]
    opacity = opacities[i % len(opacities)]
    points = layer_vertices[-1]
    dwg.add(dwg.polygon(points, fill=color, opacity=opacity, stroke="none"))
    # Prepare next layer
    next_points = grow_polygon(points, radius_increment, rotation_deg)
    layer_vertices.append(next_points)

dwg.save()
print("SVG saved as 28_layers.svg")