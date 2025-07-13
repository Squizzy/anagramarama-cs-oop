import svgwrite
import math
import random

# Parameters
width, height = 800, 557
center = (width // 2 + 20, height // 2 - 20)  # Offset to match your image
num_rings = 7
num_sides = 10
min_radius = 80
max_radius = 400

# Colors and opacities (approximate from your image)
colors = [
    "#ff9900",  # orange
    "#ffcc33",  # yellow-orange
    "#e6f97a",  # yellow-green
    "#b6e7b6",  # light green
    "#a3d3e3",  # light blue
    "#7faee3",  # blue
    "#7faee3",  # blue (outermost)
]
opacities = [0.9, 0.7, 0.7, 0.8, 0.9, 1.0, 1.0]

dwg = svgwrite.Drawing("radial_polygons.svg", size=(width, height))

for i in reversed(range(num_rings)):
    radius = min_radius + (max_radius - min_radius) * i / (num_rings - 1)
    angle_offset = (i % 2) * (math.pi / num_sides)  # alternate rotation for variety
    points = []
    for j in range(num_sides):
        angle = 2 * math.pi * j / num_sides + angle_offset
        # Add jitter for organic look
        jitter = random.uniform(-0.08, 0.08) * radius
        x = center[0] + (radius + jitter) * math.cos(angle)
        y = center[1] + (radius + jitter) * math.sin(angle) * (height / width)
        points.append((x, y))
    dwg.add(
        dwg.polygon(
            points,
            fill=colors[i % len(colors)],
            opacity=opacities[i % len(opacities)],
            stroke="none"
        )
    )

dwg.save()
print("SVG saved as radial_polygons.svg")