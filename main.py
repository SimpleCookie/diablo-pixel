import cv2
import utilities as util

original = cv2.imread('img/test.png')
bluePortal = cv2.imread('img/blue-portal.png')
a4Wp = cv2.imread('img/a4-wp.png')

portalWidth = bluePortal.shape[1]
portalHeight = portalWidth
portals = util.findEntity(original, bluePortal, portalWidth, portalHeight)

a4WpWidth = a4Wp.shape[1]
a4WpHeight = a4WpWidth*0.6
a4Wps = util.findEntity(original, a4Wp, a4WpWidth, a4WpHeight)


for (x, y, w, h) in portals:
  util.drawLoc(original, (x, y), (w, h))

for (x, y, w, h) in a4Wps:
  util.drawLoc(original, (x, y), (w, h))


util.show(original)
