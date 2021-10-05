import cv2
import numpy as np

markerColor = (0,255,0)
markerThickness = 5

def show(img):
  preview = cv2.resize(img, (0,0), fx=0.75, fy=0.75)
  cv2.imshow("preview!", preview)
  cv2.waitKey(0)

def drawLoc(img, topLeft, size):
  cv2.rectangle(img, topLeft, (topLeft[0]+size[0], topLeft[1]+size[1]), markerColor, markerThickness)

def findEntity(img, entity, w, h):
  entity = cv2.matchTemplate(img, entity, cv2.TM_CCOEFF_NORMED)
  yloc, xloc = np.where(entity >= .90)
  print("matched entities", xloc)
  recs = []
  for (x, y) in zip(xloc, yloc):
    recs.append([int(x), int(y), int(w), int(h)])
    recs.append([int(x), int(y), int(w), int(h)])

  recs, weights = cv2.groupRectangles(recs, 1, 0.2)
  print("grouped entities", len(recs))
  return recs
