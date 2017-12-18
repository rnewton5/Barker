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
var url = document.getElementById("url").value; // the url to send a request to
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
    request.open('GET', url + "?lastId=" + lastPostId);
    request.onload = function() {
        var jsonData = JSON.parse(request.responseText);
        if (request.status == 200){
            loadMorePosts(jsonData.result);
        } else {
            console.log(jsonData.message);
        }
    }
    request.send();
}

// parses JSON data for posts and displays it in the posts-container
// if no posts were recieved, it displays a message that there are no
// more posts to display, and sets 'outOfPosts' to true.
function loadMorePosts(data){
    var htmlString = "";
    if (data.length > 0){
        for(var i = 0; i < data.length; i++) {
            htmlString += buildPostHtml(data[i].bark, data[i].likesPost, data[i].following, data[i].owner);
        }
        lastPostId = data[data.length-1].bark.id;
    } else {
        htmlString = "<div><hr><p>There are no more posts to be seen.</p></div>"
        outOfPosts = true;
    }
    postsContainer.insertAdjacentHTML('beforeend', htmlString);
}

// takes a post object, a boolean dictating if the post has been liked,
//     a boolean dictating the user is following the author, and a boolean 
//     dictating if the user is the owner of the post.
function buildPostHtml(post, likesPost, following, owner) {
    var heartStyle = likesPost ? "style='color: #f00;'" : "";
    var followButton;
    var editButton;
    var deleteButton;
    if (owner) {
        var editButton = "<button class='edit-post-btn btn btn-info' value='" + post.id + "' role='button'>Edit</button>";
        var deleteButton = "<a class='delete-post-button btn btn-danger' role='button' href='/Post/DeletePost?postId=" + post.id + "'>Delete</a>"
        followButton = "";
    } else {
        var followMessage = following ? "- Unfollow" : "+ Follow";
        followButton = "<a class='follow-button btn btn-info' role='button' href='/Follow/ToggleFollow/" + post.author + "'>" + followMessage + "</a>";
        editButton = "";
        deleteButton = "";
    }
    return "<div class='post'><hr><h4>"
        + "<a class='post-author' href='/User/Profile/" + post.author + "'>"
        + post.author + "</a></h4>"
        + "<p>" + htmlEntities(post.message) + "</p><div class='pull-left'>"
        + "<a href='/Like/ToggleLike?postid=" + post.id + "' class='like-post-button' " + heartStyle + ">"
        + "<span class='glyphicon glyphicon-heart'></span></a></div>"
        + followButton + editButton + deleteButton
        + "<h6 class='pull-right post-date'>" + post.postDate + "</h6></div>"; 
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