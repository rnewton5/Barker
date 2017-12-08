/*
 * This file is used to recieve lists of posts from the server 
 *   via /Posts/GetPosts/<userName>?lastId=<Id of the last post recieved, or -1>
 * 
 * in order to get posts for a specific user, you must have a hidden field
 *   with an id of "user-name" that has a value of the user name.
 *   ex: '<input type="hidden" id="user-name" value="<userName>">
 * 
 * When the user scrolls to the bottom of the screen, a new request is sent to the server
 *   to get more posts.
 */
var postsContainer = document.getElementById("posts-container"); // the container to put the posts in
var userName = document.getElementById("user-name").value; // the username to get specific posts for. can be left blank
var url = "/Post/GetPosts/"; // the url to send a request to
var lastPostId = -1; // the id of the most recently recieved post, server disregards if -1
var LastRequestTime = 0; // used to limit the number of requests to the server
var outOfPosts = false; // prevents unnecessary requests from being made

// starts a request as soon as the file loads
(function() {
    initiateRequest();    
})();

// sends a request to the server for more posts
function initiateRequest() {
    if (outOfPosts) return;
    var request = new XMLHttpRequest();
    request.open('GET', url + userName + "?lastId=" + lastPostId);
    request.onload = function() {
        var jsonData = JSON.parse(request.responseText);
        loadMorePosts(jsonData);
    }
    request.send();
}

// parses JSON data for posts and displays it in the posts-container
// if no posts were recieved, it displays a message that there are no
// more posts to display, and sets 'outOfPosts' to true.
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

// replaces characters in a string so that it can be properly displayed and returns it.
function htmlEntities(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/\n/g, '<br>').replace(/ /g, '&nbsp;');
}

// loads more posts when the user scrolls to the bottom of the page. can only be triggered once per second
window.onscroll = function(){
    var now = Date.now();
    var dt = now - LastRequestTime;
    if ((window.innerHeight + window.pageYOffset) >= document.body.offsetHeight && this.oldScroll < this.scrollY && dt > 1000) {
        initiateRequest();
        LastRequestTime = now;
    }
    this.oldScroll = this.scrollY;
}