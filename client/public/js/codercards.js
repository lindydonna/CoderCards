console.log("loading");
// After DOM is loaded set up Dropzone/etc.
function autorun() {
    console.log("Dropzone configured");

    var myDropzone = new Dropzone("form#dropzone", {
        autoQueue: false,
        url: (files) => {
            return `https://chrande-codercards.azurewebsites.net/upload/${files[0].name}`;
        },
        headers: {
            "x-ms-blob-type": "BlockBlob",
            "Content-Type": "image/jpeg"
        },
        clickable: true,
        method: "PUT",
        init: function () {
            this.on("success", function (file, response) {
                console.log(file.name);
            });
            this.on("addedfile", function (file) {
                uploadFile(file);
                const that = this;
                setTimeout(() => {
                    that.removeFile(file);
                }, 1000);
                enqueueCard(file.name);
            });
        }

    });
}
if (window.addEventListener) window.addEventListener("load", autorun, false);
else if (window.attachEvent) window.attachEvent("onload", autorun);
else window.onload = autorun;

function uploadFiles() {
    const files = document.getElementById("fileToUpload").files;
    for (const file of files) {
        uploadFile(file);
    }
}

function enqueueCard(name) {
    const images = document.getElementById("images");
    const imageDiv = document.createElement("div");
    imageDiv.classList.add("card");
    const loading = document.createElement("span");
    loading.classList.add("loading");
    loading.classList.add("fa");
    loading.classList.add("fa-refresh");
    imageDiv.appendChild(loading);
    images.appendChild(imageDiv);
    imageDiv.id = `image-${name}`;
    let interval = setInterval(() => {
        console.log("Looking for file");
        getImage(name, (done) => {
            if (done) {
                console.log("Clearing interval");
                clearInterval(interval);
            }
        });
    }, 10000);
}

function uploadFile(file) {
    const xhr = new XMLHttpRequest();
    xhr.open("PUT", `https://chrande-codercards.azurewebsites.net/upload/${file.name}`);
    xhr.setRequestHeader('x-ms-blob-type', 'BlockBlob');
    xhr.setRequestHeader('Content-Type', 'image/jpeg');
    xhr.send(file);
}

function getImage(name, cb) {
    const xhr = new XMLHttpRequest();
    const imageUrl = `https://chrande-codercards.azurewebsites.net/card/${name}`;
    xhr.open("GET", imageUrl);
    xhr.responseType = "blob";
    xhr.onload = (e) => {
        if (xhr.status != 200) {
            console.log("Image does not exist yet");
            return cb(false);
        }
        let images = document.getElementById(`image-${name}`);
        images.innerHTML = '';
        let image = document.createElement('img');
        image.src = imageUrl;
        image.setAttribute("height", "300px");
        images.appendChild(image);
        console.log("Image created");
        return cb(true);
    }
    var results = xhr.send();
}