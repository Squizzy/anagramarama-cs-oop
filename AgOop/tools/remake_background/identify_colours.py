from PIL import Image

image = Image.open("background.png")

colours = image.getcolors(maxcolors=254)

print(len(colours))