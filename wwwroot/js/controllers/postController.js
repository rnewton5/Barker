var postsContainer = document.getElementById("posts-container");
var userName = document.getElementById("user-name").value;
var lastPostId = -1;
var LastRequestTime = 0;
var outOfPosts = false;

// starts a request as soon as the file loads
(function() {
    initiateRequest();    
})();

// sends a request to the server for more posts
function initiateRequest() {
    if (outOfPosts) return;
    var request = new XMLHttpRequest();
    request.open('GET', "/Post/GetPosts/" + userName + "?lastId=" + lastPostId);
    request.onload = function() {
        var jsonData = JSON.parse(request.responseText);
        loadMorePosts(jsonData);
    }
    request.send();
}

// takes JSON data from Post/getPosts and displays it in the posts-container
function loadMorePosts(data){
    var htmlString = "";
    if (data.barks.length > 0){
        for(i = 0; i < data.barks.length; i++) {
            htmlString += "<div class='post'><hr><h4>"
                + "<a href='User/Profile/" + data.barks[i].author + "'>"
                + data.barks[i].author + "</a></h4>"
                + "<p>" + htmlEntities(data.barks[i].message) + "</p>"
                + "<h6 class='pull-right post-date'>" + data.barks[i].postDate + "</h6></div>";
        }
        lastPostId = data.barks[data.barks.length-1].id;
    } else {
        htmlString = "<div><hr><p>There is no more to be seen.</p></div>"
        outOfPosts = true;
    }
    postsContainer.insertAdjacentHTML('beforeend', htmlString);
}

// encodes a string of characters so that it can be safely displayed.
function htmlEntities(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/\n/g, '<br>').replace(/ /g, '&nbsp;');
}

// loads more posts when the user scrolls to the bottom of the page. can only be triggered once per second
var sendAnotherRequest = function(){
    var now = Date.now();
    var dt = now - LastRequestTime;
    if ((window.innerHeight + window.pageYOffset) >= document.body.offsetHeight && this.oldScroll < this.scrollY && dt > 1000) {
        initiateRequest();
        LastRequestTime = now;
    }
    this.oldScroll = this.scrollY;
}

window.onscroll = sendAnotherRequest;