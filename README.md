# Earthquake Response Visualizer (ERV)

### Install
Run `npm install` to install libraries


#### Convert data sets
##### convert building data
- replace building data set file `building_data.json` in data folder
- run command `node server/conversion/convert-buildingdata2kml.js` to convert to kml

##### convert sensor data
- place sensor data set file in data folder
- update the data set file name DATASET in `server/conversion/convert-quakedata2kml.js` script
- run command `node server/conversion/convert-quakedata2kml.js` to convert
