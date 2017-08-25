// ==UserScript==
// @name        GitHubReleasesCount
// @namespace   net.r_eg.GitHubReleasesCount
// @version     0.1
// @grant       none
// @include     https://github.com/*/*/releases
// @require     https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js
// @author      https://github.com/3F
// ==/UserScript==

/* Tested via: Firefox 55.0.2 & 54.0.1 + Greasemonkey 3.11 */
$(function()
{
    //github.com/<user>/<project>/releases
    var ghr = window.location.href.replace('github.com/', 'api.github.com/repos/');

    $.get(ghr, function(apidata)
    {
        $(".release-downloads li a").each(function()
        {
            var root = $(this);
            var durl = root.attr('href');

            if(durl.indexOf('releases/download') == -1) {
                return true; // means 'continue' statement
            }

            for(var idx in apidata)
            for(var asset in apidata[idx].assets)
            {
                var lnk = apidata[idx].assets[asset];

                if(!lnk.browser_download_url.endsWith(durl)) {
                    continue;
                }

                $("<span class='release-label latest'>" + lnk.download_count + "</span>")
                    .insertBefore(root);
            }
        });
    });
});