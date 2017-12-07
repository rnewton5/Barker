var postsContainer = document.getElementById("posts-container");

(function() {
    var request = new XMLHttpRequest();
    request.open('GET', "Post/GetPosts");
    request.onload = function() {
        var jsonData = JSON.parse(request.responseText);
        initialLoad(jsonData);
    }
    request.send();
})();

function initialLoad(data){
    if (data.barks.length > 0){
        var htmlString = "";
        for(i = 0; i < data.barks.length; i++) {
            htmlString += "<div class='post'><hr><h4>"
                + "<a href='User/Profile/" + data.barks[i].author + "'>"
                + data.barks[i].author + "</a></h4>"
                + "<p>" + data.barks[i].message + "</p>"
                + "<h6 class='pull-right post-date'>" + data.barks[i].postDate + "</h6></div>";
        }
        postsContainer.removeChild(postsContainer.children[0]);
        postsContainer.insertAdjacentHTML('beforeend', htmlString);
    }
}