import './style.css';
import { toastrSuccess, toastrError, toastrInfo} from './notification.mjs';
import { Map, View } from 'ol';
import Feature from 'ol/Feature';
import TileLayer from 'ol/layer/Tile';
import { fromLonLat, toLonLat } from 'ol/proj';
import OSM from 'ol/source/OSM';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import { Point } from 'ol/geom';
import Style from 'ol/style/Style';
import Icon from 'ol/style/Icon';
import Draw from 'ol/interaction/Draw';
import Modify from 'ol/interaction/Modify';
import Fill from 'ol/style/Fill';
import Stroke from 'ol/style/Stroke';
import Snap from 'ol/interaction/Snap';
import { Attribution, defaults as defaultControls } from 'ol/control';
import Overlay from 'ol/Overlay';
import WKT from 'ol/format/WKT';
import { ColorType } from 'ol/expr/expression';



const container = document.getElementById('popup');
const content = document.getElementById('popup-content');
const closer = document.getElementById('popup-closer');

let modify;

const overlay = new Overlay({
  element: container,
  autoPan: {
    animation: {
      duration: 250,
    },
  },
});

closer.onclick = function () {
  overlay.setPosition(undefined);
  closer.blur();
  return false;
};

const raster = new TileLayer({
  source: new OSM(),
});

var source = new VectorSource({
});

const vector = new VectorLayer({
  source: source,
});

const map = new Map({
  layers: [raster, vector],
  controls: defaultControls({ attribution: false }),
  target: 'map',
  view: new View({
    center: fromLonLat([35.1667, 38.9967]),
    zoom: 7,
  }),
});

map.addOverlay(overlay);

let shouldShowPopup = true;

//popout
map.on('click', function (evt) {
  if (!shouldShowPopup) return;
  const feature = map.forEachFeatureAtPixel(evt.pixel, function (feature) {
    return feature;
  });

  if (feature) {
    console.log('Feature clicked properties:', feature.getProperties());

    const geometry = feature.getGeometry();
    const geometryType = geometry.getType();
    let coordinates;

    if (geometryType === 'Point') {
      coordinates = geometry.getCoordinates();
    } else if (geometryType === 'Polygon') {
      coordinates = geometry.getInteriorPoint().getCoordinates();
    } else {
      console.error(`Unsupported geometry type: ${geometryType}`);
      return;
    }

    const pointName = feature.get('name');
    const wktFromDb = feature.get('wkt');
    const id = feature.get('id');
    console.log('Feature clicked:', { pointName, wktFromDb, id });

    content.innerHTML = `
      <hr>
      <p>Name: <input type="text" id="update-name" value="${pointName}"></p>
      <p>WKT: <input type="text" id="update-wkt" value="${wktFromDb}"></p>
      <button id="update-point" style="background-color:orange;margin:5px;">Update</button>
      <button id="manuel-update-point" style="background-color:gray;margin:5px;">Update Manualy</button>
      <button id="delete-point" style="background-color:red;color:white;margin:5px;">Delete</button>
      <hr>
`;
    overlay.setPosition(coordinates);

    document.getElementById('update-point').addEventListener('click', async function () {
      const newName = document.getElementById('update-name').value;
      const newWkt = document.getElementById('update-wkt').value;
    
      const updatedPoint = {
        id: feature.get('id'),
        name: newName,
        wkt: newWkt,
      };
    
      try {
        const response = await fetch(`https://localhost:7235/api/Point/${updatedPoint.id}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(updatedPoint)
        });
    
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
    
        const result = await response.json();
        console.log('Point updated successfully:', result);
        toastrSuccess(result.message);
    
        source.clear();
        getPoints();
        overlay.setPosition(undefined);
    
      } catch (error) {
        toastrError('Error updating point:', error);
        console.error('Error updating point:', error);
      }
    });

    document.getElementById('delete-point').addEventListener('click',async function () {
      const id = feature.get('id')

      try {
        const response = await fetch(`https://localhost:7235/api/Point/Delete/${id}`, {
          method: 'DELETE',
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();
        console.log('Point deleted successfully:', result);
        toastrSuccess(result.message);

        source.clear();
        getPoints();
        overlay.setPosition(undefined);

      } catch (error) {
        console.error('Error deleting point:', error);
      }
    })

    document.getElementById('manuel-update-point').addEventListener('click',function () {
      const modify = new Modify({
        source: source,
        hitDetection: vector,
      });
      map.addInteraction(modify);
      overlay.setPosition(undefined);
      
      const geometry = feature.getGeometry();
      const geometryType = geometry.getType();
      let updatedPoint;

      //Update by Moving
      if (geometryType == 'Point') {
        modify.on('modifyend', async function (evt) {
            const coordinates = feature.getGeometry().getCoordinates();
            const [lon, lat] = toLonLat(coordinates); 
        
            const format = new WKT();
        
            const pointGeometry = new Point([parseFloat(lon), parseFloat(lat)]);
            const wktPoint = format.writeGeometry(pointGeometry);
  
            updatedPoint = {
              id: feature.get('id'),
              name: feature.get('name'), 
              wkt: wktPoint,
            };
          
            try {
              const response = await fetch(`https://localhost:7235/api/Point/${updatedPoint.id}`, {
                method: 'PUT',
                headers: {
                  'Content-Type': 'application/json'
                },
                body: JSON.stringify(updatedPoint)
              });
        
              if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
              }
        
              const result = await response.json();
              console.log('Point updated successfully:', result);
              toastrSuccess(result.message);
              map.removeInteraction(modify);
              getPoints();
        
            } catch (error) {
              console.error('Error updating point:', error);
            }
          
        });
      } else if (geometryType == 'Polygon') {
        const modify = new Modify({
          source: source,
          hitDetection: vector,
        });
  
        map.addInteraction(modify);
        overlay.setPosition(undefined);
  
        let panelOpened = false;

        if (geometryType == 'Polygon') {
          modify.on('modifyend', function (evt) {
            console.log('Editing finished, click inside the polygon to open the panel');
            panelOpened = false;
          });
        }
  
        shouldShowPopup = false;
      
        map.on('singleclick', function (evt) {
          map.forEachFeatureAtPixel(evt.pixel, function (feature) {
            if (feature.getGeometry().getType() === 'Polygon' && !panelOpened) {
              const coordinates = feature.getGeometry().getCoordinates();
              const format = new WKT();
              const polygonGeometry = feature.getGeometry();
              const wktPolygon = format.writeGeometry(polygonGeometry);
  
              showJsPanelForModify({
                id: feature.get('id'),
                name: feature.get('name'),
                wkt: wktPolygon,
              });
              panelOpened = true;
            }
          });
        });
      } else {
        overlay.setPosition(undefined);
      }
      
    });
  } 
});

let panelShowJsPanleForModify;
//for showing panel upon clicking in area 
function showJsPanelForModify(updatedPoint) {
  panelShowJsPanleForModify = jsPanel.create({
    headerTitle: 'Edit Polygon',
    contentSize: '400 200',
    content: `
      <div>
        <label for="edit-name">Name:</label>
        <input type="text" id="edit-name" value="${updatedPoint.name}" />
      </div>
      <div style="margin-top: 20px;">
        <button id="save-button">Save</button>
      </div>
    `,
    callback: function (panel) {
      this.content.style.padding = "15px";
      const saveButton = panel.content.querySelector('#save-button');
      saveButton.addEventListener('click', async function () {
        const nameInput = panel.content.querySelector('#edit-name');
        updatedPoint.name = nameInput.value;

        panelShowJsPanleForModify.close();

        try {
          const response = await fetch(`https://localhost:7235/api/Point/${updatedPoint.id}`, {
            method: 'PUT',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(updatedPoint),
          });

          if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
          }

          const result = await response.json();
          console.log('Polygon updated successfully:', result);
          toastrSuccess(result.message);
          map.removeInteraction(modify);
          getPoints();
          shouldShowPopup = true;

          // panel.close();
          return;

        } catch (error) {
          console.error('Error updating polygon:', error);
        }
      });
    },
  });
}

//get elements from database
async function getPoints() {
  try {
    const response = await fetch('https://localhost:7235/api/Point/GetAllJSON');

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const jsonResponse = await response.json();
    const elements = jsonResponse.data;

    elements.forEach((element) => {

      const format = new WKT();
      let feature;

      if (element.wkt.includes("POINT")) {
        feature = format.readFeature(element.wkt, {
          dataProjection: 'EPSG:4326', 
          featureProjection: 'EPSG:3857'
        });

        const pointStyle = new Style({
          image: new Icon({
            anchor: [0.5, 1],
            src: './location-pin.png',
            scale: 0.8
          }),
        });
        feature.setStyle(pointStyle);
      } else if (element.wkt.includes("POLYGON")) {
        feature = format.readFeature(element.wkt);

        const polygonStyle = new Style ({
          stroke: new Stroke({
            color: 'blue',
            width: 2
          }),
          fill: new Fill({
            color: 'rgba(0, 0, 255, 0.1)'
          })
        });
        feature.setStyle(polygonStyle);
      }
      if (feature) {
        feature.setId(element.id);
        feature.set("id", element.id);
        feature.set("wkt", element.wkt);
        feature.set("name", element.name);
        source.addFeature(feature);
      }
    });
  } catch (error) {
    toastrError('Error fetching points!');
    console.error('Error fetching points:', error);
  }
}

let interactionEnabled = false;
let drawingMode = 'none'; 
let draw; 

const addPointButton = document.getElementById('addPoint');
const addPolygonButton = document.getElementById('addArea');

addPointButton.addEventListener('click', () => {
  if (drawingMode === 'point') {
    toastrInfo("Adding points deactivated.");
    disableInteraction();
    setButtonState(addPointButton, false);
    console.log("Should Show PopUp?: "+ shouldShowPopup);
    shouldShowPopup = true;
  } else {
    toastrInfo("Adding points activated.");
    drawingMode = 'point';
    enablePointInteraction();
    setButtonState(addPointButton, true);
    setButtonState(addPolygonButton, false);
    console.log("Should Show PopUp?: "+ shouldShowPopup);
    shouldShowPopup = false;
  }
});

addPolygonButton.addEventListener('click', () => {
  if (drawingMode === 'polygon') {
    toastrInfo("Adding areas deactivated.");
    disableInteraction();
    setButtonState(addPolygonButton, false);
    shouldShowPopup = true;
  } else {
    toastrInfo("Adding areas activated.");
    drawingMode = 'polygon';
    enablePolygonInteraction();
    setButtonState(addPolygonButton, true);
    setButtonState(addPointButton, false);
    shouldShowPopup = false;
  }
});

function setButtonState(button, isActive) {
  if (isActive) {
    button.classList.add('active');
    button.classList.remove('inactive');
  } else {
    button.classList.add('inactive');
    button.classList.remove('active');
  }
}

function enablePointInteraction() {
  interactionEnabled = true;
  shouldShowPopup = true;
  if (draw) {
    map.removeInteraction(draw);
    draw = null;
  }
}

function enablePolygonInteraction() {
  interactionEnabled = false;
  addInteractions();
  shouldShowPopup = false;
}

function disableInteraction() {
  interactionEnabled = false;
  drawingMode = 'none';
  shouldShowPopup = false;
  if (draw) {
    map.removeInteraction(draw);
    draw = null;
  }
}

function addInteractions() {
  if (drawingMode === 'polygon') {
    draw = new Draw({
      source: source,
      type: 'Polygon'
    });

    draw.on('drawend', function(evt) {
      const format = new WKT();
      const wktPolygon = format.writeGeometry(evt.feature.getGeometry());

      const panel = jsPanel.create({
        theme: "white",
        contentSize: { width: 350, height: 160 },
        headerTitle: "Area Name",
        content: `
          <div style="padding: 15px;">
            <label for="areaName">Name:</label>
            <input type="text" id="areaName" placeholder="Enter name of the area"><br><br>
            <button id="saveAreaName">Save</button>
          </div>
        `,
        callback: function () {
          this.content.style.padding = "15px";

          document.getElementById('saveAreaName').addEventListener('click', async function () {
            const name = document.getElementById('areaName').value;
            if (name.trim() === '') {
              alert('Please enter a valid name for the area.');
              return;
            }

            const polygon = {
              wkt: wktPolygon,
              name: name
            };

            panel.close();

            savePolygon(polygon);
          });
        }
      });
    });

    map.addInteraction(draw);
  }
}

async function savePolygon(polygon,name) {
  try {
    
    const response = await fetch('https://localhost:7235/api/Point', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(polygon)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    console.log('Polygon saved successfully:', result);
    toastrSuccess(result.message);

    getPoints();

  } catch (error) {
    console.error('Error saving polygon:', error);
  }
}


let lat;
let lon;
//add point
map.on('click', function (evt) {
  if (!interactionEnabled || drawingMode !== 'point') return;
  const coords = toLonLat(evt.coordinate);
  lat = coords[1];
  lon = coords[0];

  const panel = jsPanel.create({
    theme: "white",
    contentSize: { width: 350, height: 260 },
    headerTitle: "Add Point",
    content: `
      <div style="padding: 15px;">
        <label for="x">X:</label>
        <input type="text" id="x" name="x" value="${lon}" readonly><br><br>
        <label for="y">Y:</label>
        <input type="text" id="y" name="y" value="${lat}" readonly><br><br>
        <label for="name">Name:</label>
        <input type="text" id="name" name="name"><br><br>
        <button id="savepoint">Save</button>
      </div>
    `,
    callback: function () {
      this.content.style.padding = "15px";

      document.getElementById('savepoint').addEventListener('click', async function () {
        const name = document.getElementById('name').value;
        const x = document.getElementById('x').value;
        const y = document.getElementById('y').value;

        const format = new WKT();

        const pointGeometry = new Point([parseFloat(x), parseFloat(y)]);
        const wktPoint = format.writeGeometry(pointGeometry);

        const point = {
          wkt: wktPoint,
          name: name
        };

        panel.close();

        try {
          const response = await fetch('https://localhost:7235/api/Point', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify(point)
          });

          if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
          }

          const result = await response.json();
          console.log('Point saved successfully:', result);
          toastrSuccess(result.message);

          getPoints();

        } catch (error) {
          console.error('Error saving point:', error);
        }
      });
    }
  });
});

let queryPanel;
document.getElementById('query').addEventListener('click', async function () {

  queryPanel = jsPanel.create({
    theme: "white",
    contentSize: { width: 760, height: 580 },
    headerTitle: "Draws",
    content: `
      <div id="data-table-container" style="padding: 15px;">
        <table id="data-table" style="width: 100%; border-collapse: collapse;">
          <thead>
            <tr>
              <th></th>
              <th>ID</th>
              <th>Name</th>
              <th>Wkt</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody id="data-table-body">
            <!-- Rows -->
          </tbody>
        </table>
      </div>
    `,
    callback: function () {
      fetchAndDisplayData();
    }
  });
});


getPoints();

function handleViewClick(event) {
  const id = event.target.getAttribute('data-id');
  const feature = source.getFeatureById(id);
  console.log(feature);

  if (!feature) {
    console.error(`Feature with ID ${id} not found`);
    return;
  }

  const geometry = feature.getGeometry();
  const geometryType = geometry.getType();
  let coordinates;

  if (geometryType === 'Point') {
    coordinates = geometry.getCoordinates();
  } else if (geometryType === 'Polygon') {
    coordinates = geometry.getInteriorPoint().getCoordinates();
  } else {
    console.error(`Unsupported geometry type: ${geometryType}`);
    return;
  }

  if (!coordinates) {
    console.error('Coordinates not found for the feature');
    return;
  }

  console.log(`Zooming to coordinates: ${coordinates}`);
  queryPanel.close();

  map.getView().animate({
    center: coordinates,
    zoom: 12,
    duration: 1000 
  }, {
    onAnimationEnd: function() {
      console.log('Zoom animation completed');
    }
  });
}

function handleUpdateClick(event) {
  const id = event.target.getAttribute('data-id');
  const feature = source.getFeatureById(id);

  if (feature) {
    const coordinates = feature.getGeometry().getCoordinates();
    const pointName = feature.get('name');
    const wkt = feature.get('wkt');

    jsPanel.create({
      theme: "white",
      contentSize: { width: 400, height: 300 },
      headerTitle: "Update Marker",
      content: `
        <div style="padding: 15px;">
          <label for="update-name">Name:</label>
          <input type="text" id="update-name" value="${pointName}"><br><br>
          <label for="update-wkt">Wkt:</label>
          <input type="text" id="update-wkt" value="${wkt}"><br><br>
          <button id="update-marker">Update</button>
        </div>
      `,
      callback: function () {
        document.getElementById('update-marker').addEventListener('click', async function () {
          const newName = document.getElementById('update-name').value;
          const newWkt = document.getElementById('update-wkt').value;

          console.log(newWkt);

          const updatedPoint = {
            id: id,
            wkt: newWkt,
            name: newName,
          };

          try {
            const response = await fetch(`https://localhost:7235/api/Point/${updatedPoint.id}`, {
              method: 'PUT',
              headers: {
                'Content-Type': 'application/json'
              },
              body: JSON.stringify(updatedPoint)
            });

            if (!response.ok) {
              throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();
            console.log('Point updated successfully:', result);
            toastrSuccess(result.message);

            fetchAndDisplayData(); 
            source.clear();
            getPoints(); 

          } catch (error) {
            console.error('Error updating point:', error);
          }
        });
      }
    });
  }
}


function handleDeleteClick(event) {
  const id = event.target.getAttribute('data-id');

  if (confirm('Are you sure you want to delete this marker?')) {
    fetch(`https://localhost:7235/api/Point/Delete/${id}`, {
      method: 'DELETE'
    })
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    })
    .then(result => {
      console.log('Point deleted successfully:', result);
      toastrSuccess(result.message);
      fetchAndDisplayData(); 
      source.clear();
      getPoints();
    })
    .catch(error => {
      console.error('Error deleting point:', error);
    });
  }
}

function handleManualUpdateClick(event) {

  
  const id = event.target.getAttribute('data-id');
  const feature = source.getFeatureById(id);
  console.log(feature);

  if (!feature) {
    console.error(`Feature with ID ${id} not found`);
    return;
  }

  const geometry = feature.getGeometry();
  const geometryType = geometry.getType();
  let coordinates;

  if (geometryType === 'Point') {
    coordinates = geometry.getCoordinates();
  } else if (geometryType === 'Polygon') {
    coordinates = geometry.getInteriorPoint().getCoordinates();
  } else {
    console.error(`Unsupported geometry type: ${geometryType}`);
    return;
  }

  if (!coordinates) {
    console.error('Coordinates not found for the feature');
    return;
  }

  console.log(`Zooming to coordinates: ${coordinates}`);
  queryPanel.close();

  map.getView().animate({
    center: coordinates,
    zoom: 12,
    duration: 1000 
  }, {
    onAnimationEnd: function() {
      console.log('Zoom animation completed');
    }
  });

  const modify = new Modify({
    source: source,
    hitDetection: vector,
  });

  let updatedPoint;

  map.addInteraction(modify);

  overlay.setPosition(undefined);

  if (geometryType == 'Point') {
    modify.on('modifyend', async function (evt) {
        const coordinates = feature.getGeometry().getCoordinates();
        const [lon, lat] = toLonLat(coordinates); 
    
        const format = new WKT();
    
        const pointGeometry = new Point([parseFloat(lon), parseFloat(lat)]);
        const wktPoint = format.writeGeometry(pointGeometry);

        updatedPoint = {
          id: feature.get('id'),
          name: feature.get('name'), 
          wkt: wktPoint,
        };
      
        try {
          const response = await fetch(`https://localhost:7235/api/Point/${updatedPoint.id}`, {
            method: 'PUT',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify(updatedPoint)
          });
    
          if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
          }
    
          const result = await response.json();
          console.log('Point updated successfully:', result);
          toastrSuccess(result.message);
          map.removeInteraction(modify);
          getPoints();
    
        } catch (error) {
          console.error('Error updating point:', error);
        }
      
    });
  } else if (geometryType == 'Polygon') {
    const modify = new Modify({
      source: source,
      hitDetection: vector,
    });

    map.addInteraction(modify);
    overlay.setPosition(undefined);

    if (geometryType == 'Polygon') {
      modify.on('modifyend', function (evt) {
        console.log('Editing finished, click inside the polygon to open the panel');
      });
    }

    shouldShowPopup = false;
  
    map.on('singleclick', function (evt) {
      map.forEachFeatureAtPixel(evt.pixel, function (feature) {
        if (feature.getGeometry().getType() === 'Polygon') {
          const coordinates = feature.getGeometry().getCoordinates();
          const format = new WKT();
          const polygonGeometry = feature.getGeometry();
          const wktPolygon = format.writeGeometry(polygonGeometry);

          showJsPanelForModify({
            id: feature.get('id'),
            name: feature.get('name'),
            wkt: wktPolygon,
          });
        }
      });
    });
  } else {
    overlay.setPosition(undefined);
  }
}

async function fetchAndDisplayData() {
  try {
    const response = await fetch('https://localhost:7235/api/Point/GetAllJSON');

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const jsonResponse = await response.json();

    if (!jsonResponse.data || !Array.isArray(jsonResponse.data)) {
      throw new Error('Unexpected response structure');
    }

    const points = jsonResponse.data;
    const tableBody = document.getElementById('data-table-body');
    tableBody.innerHTML = ''; 

    points.forEach(point => {
      const row = document.createElement('tr');
      row.innerHTML = `
      <div style="padding-bottom:20px;">
        <td>${point.id}</td>
        <td>${point.name}</td>
        <td class="truncated">${point.wkt}</td>
        <td>
          <button style="background-color:blue;color:white"  class="view-button" data-id="${point.id}">View</button>
          <button style="background-color:orange"  class="update-button" data-id="${point.id}">Update</button>
          <button style="background-color:gray;color:white" class="manual-update-button" data-id="${point.id}">Manual Update</button>
          <button style="background-color:red;color:white" class="delete-button" data-id="${point.id}">Delete</button>
        </td>
      </div> 
      `;
      tableBody.appendChild(row);
    });

    document.addEventListener("DOMContentLoaded", function() {
      const cells = document.querySelectorAll('.truncated');
      cells.forEach(cell => {
        const maxLength = 20;
        const text = cell.textContent || cell.innerText;
        if (text.length > maxLength) {
          cell.textContent = text.substring(0, maxLength) + '...';
        }
      });
    });

    document.querySelectorAll('.view-button').forEach(button => {
      button.addEventListener('click', handleViewClick);
    });
    document.querySelectorAll('.update-button').forEach(button => {
      button.addEventListener('click', handleUpdateClick);
    });
    document.querySelectorAll('.manual-update-button').forEach(button => {
      button.addEventListener('click', handleManualUpdateClick);
    });
    document.querySelectorAll('.delete-button').forEach(button => {
      button.addEventListener('click', handleDeleteClick);
    });

  } catch (error) {
    console.error('Error fetching data:', error);
  }
}