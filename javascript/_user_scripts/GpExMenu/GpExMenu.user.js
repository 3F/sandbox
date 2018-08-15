// ==UserScript==
// @name        GpExMenu
// @description Alpha Non-API version.
// @namespace   net.r_eg.GpExMenu
// @version     0.0.2
// @grant       none
// @include     https://plus.google.com/*
// @author      https://github.com/3F
// ==/UserScript==

/**
 * Draft. Non-API version
 * ...
 * v0.0.2 - Alpha. Fixed tracking between AunVZ, lNccYc, MydTse controllers.
 * v0.0.1 - Alpha Non-API version. 
 */
(function()
{
    'use strict';

    class GpExMenu extends function(){ this.constructor.prototype.$ =
    {
        idents:
        {
            /** Button to call {idents.btnmenu} */
            btnmenu: '[jscontroller="iSvg6e"]',

            /** Region to understand when will be received {idents.rcvmenu}. */
            trackChanges: 'c-wiz[jscontroller="AunVZ"], [jscontroller="lNccYc"], [jscontroller="MydTse"]',

            /** Container of {trackChanges}. Because it may be replaced by adding new without removing prev. */
            trackContainer: 'body',

            /** nodeName of updated container from {trackContainer} */
            updatedContainer: 'C-WIZ',

            /** Top line of the post. timestamp, [:] */
            headerPost: '[jscontroller="ugNwKb"]',

            /** Received post */
            rcvpost: 'uVLKOc',

            /** Received menu after {idents.btnmenu} ~ jscontroller */
            rcvmenu: 'uY3Nvd',

            /** Menu item to ignore post. */
            actIgnore: '[jsaction*="JIbuQc:OpVP7b"]',

            /** To add event for activation {idents.actIgnore} item. */
            actIgnoreClk: 'click:OpVP7b;',

            /** Menu item to restore post. */
            actRestore: '[jsaction*="JIbuQc:IcGNMd"]',

            /** To add event for activation {idents.actRestore} item. */
            actRestoreClk: 'click:IcGNMd;',

            /** Marks our processing for posts */
            ownPostLabel: 'snYNbsp',
        },

        /** delay in ms before click */
        delayClick: 750,

        debug: false,

        /* --------------- */ }}{ /* --------------- */

        constructor(debug)
        {
            super();

            this.$.debug = debug;
            this.dbg('GpExMenu ctor');

            this.$.idents.ownPostLabel = this.azgen(7);
            this.dbg('Generated attr code: ', this.$.idents.ownPostLabel);

            this.updateControls(null);
            this.observeContainer(this.$.idents.trackContainer);
        }
        
        process(scope)
        {
            this.dbg('Started for: ' + location.pathname);

            let _= this;
            _.observeMenu(_.$.idents.trackChanges, function(mutation)
            {
                _.dbg(_.$.idents.trackChanges + ' mutation: ', mutation);

                try {
                    return _.tune(mutation, scope);
                }
                catch(ex) {
                    _.dbg('Error while processing: ', ex);
                }
                return false;
            });

            _.dbg('Click for ', _.$.idents.btnmenu, scope);
            _.getElem(_.$.idents.btnmenu, scope).click();
        }

        /**
         * 
         * @param {MutationObserver} mutation 
         * @param {NodeList} Initial scope
         * @returns {boolean} True value if processing is done.
         */
        tune(mutation, scope)
        {
            let [_, rcvmenu] = [this, null];

            for(let added of mutation.addedNodes)
            {
                if('getAttribute' in added && 
                    added.getAttribute('jscontroller') == _.$.idents.rcvmenu)
                {
                    rcvmenu = added;
                    _.dbg('Found menu from mutation: ', rcvmenu);
                    break;
                }
            }

            if(_.isNull(rcvmenu)){
                _.dbg('Menu was not found.');
                return false;
            }

            rcvmenu.setAttribute('style', 'display:none;');

            let [elem, act] = [null, null];
            let toIgnore    = !scope.hasAttribute(_.$.idents.ownPostLabel) 
                                || scope.getAttribute(_.$.idents.ownPostLabel) == 'false';

            if(toIgnore) {
                elem    = _.getElem(_.$.idents.actIgnore);
                act     = _.$.idents.actIgnoreClk;
            }
            else {
                elem    = _.getElem(_.$.idents.actRestore);
                act     = _.$.idents.actRestoreClk;
            }

            try
            {
                _.setPostView(scope, toIgnore);
            }
            catch(ex) {
                _.dbg('Failed when changing view ', toIgnore, scope);
            }            

            _.useMenu(rcvmenu, function()
            {
                if(elem.getAttribute('jsaction').indexOf(act) == -1)
                {
                    _.dbg('Updated jsaction + ' + act, elem);
                    elem.setAttribute('jsaction', act + elem.getAttribute('jsaction'));
                }
                _.dbg('Click for ' + act, elem);
                elem.click();

                _.dbg('==== Action "' + act + '" is activated ====');
            });
            
            return true;
        }

        /**
         * 
         * @param {object} region 
         * @returns {MutationObserver}
         */
        observePosts(region)
        {
            if(this.isNull(region)) {
                throw new GpmNullException('region'); 
            }

            let cfg = { childList: true, subtree: true };

            let _= this;
            let mut = new MutationObserver(function(mutations)
            {
                for(let m of mutations)
                {
                    if(_.isNull(m.addedNodes) || m.addedNodes.length < 1) {
                        continue;
                    }

                    _.dbg('New possible posts:', m);
                    mut.disconnect();
                    try {
                        _.addControls(m.addedNodes);
                    }
                    catch(ex) {
                        _.dbg('Failed observePosts: ', ex);
                    }
                    mut.observe(region, cfg);
                }
            });

            _.dbg('Control of posts is started: ', region);
            mut.observe(region, cfg);
            return mut;
        }

        /**
         * 
         * @param {string} region 
         * @returns {MutationObserver}
         */
        observeContainer(region)
        {
            let _= this;

            let nodes = function(added)
            {
                if(_.isNull(added)) {
                    return;
                }

                _.dbg('New possible container: ', region);
                added.forEach(function(elem)
                {
                    if(elem.nodeName == _.$.idents.updatedContainer 
                        && elem.parentNode.nodeName == 'BODY')
                    {
                        _.dbg("Found new container " + _.$.idents.updatedContainer, elem);
                        _.updateControls(elem);
                    }
                });
            }

            let obj = new MutationObserver(function(mutations)
            {
                mutations.forEach(function(m)
                {
                    try
                    {
                        nodes(m.addedNodes);
                    }
                    catch(ex) {
                        _.dbg("Failed when container is updated", ex);
                    }
                });
            });

            _.dbg('Observed container: ', region);
            obj.observe(_.getElem(region), { childList: true, subtree: true });

            return obj;
        }
        
        observeMenu(region, action)
        {
            if(this.isNull(action)) {
                throw new GpmNullException('action');
            }

            let _= this;
            return _.observeOn(_.getLastElem(region), function(m)
            {
                if(_.isNull(m.addedNodes) || m.addedNodes.length < 1) {
                    return true;
                }

                _.dbg("Something has been received. Ready for action.", m);
                return action(m);
            }, 
            { childList: true, subtree: true });
        }
        
        useMenu(region, action)
        {
            if(this.isNull(action)) {
                throw new GpmNullException('action');
            }

            let _= this;

            // let obj = _.observeOn(region, function(m)
            // {
            //     _.dbg('Menu is ready', m);
            //     _.observeOff(obj);
            //     action();
            //     return true;
            // }, 
            // { attributes: true, characterData: true });

            setTimeout(action, _.$.delayClick);
            return null;
        }
        
        observeOn(region, action, cfg)
        {
            if(!cfg || this.isNull(region) || this.isNull(action)) {
                throw new GpmNullException('cfg, region, action');
            }

            let _= this;
            let obj = new MutationObserver(function(mutations)
            {
                mutations.forEach(function(m)
                {
                    if(action(m)) {
                        return;
                    }
                });
                _.observeOff(obj);
            });

            _.dbg('Observed: ', region);
            obj.observe(region, cfg);

            return obj;
        }
        
        observeOff(obj)
        {
            if(this.isNull(obj)) {
                //throw new GpmNullException('obj');
                this.dbg('Observer is null.');
                return;
            }

            this.dbg('Disabling observe: ', obj);
            obj.disconnect();
            obj = null;
        }

        updateControls(scope)
        {
            for(let elem of this.getElem(this.$.idents.headerPost, scope, true)) {
                this.addControl(elem);
            }

            this.observePosts(
                this.getLastElem(this.$.idents.trackChanges)
            );
        }
        
        addControl(elem)
        {
            let btnHide = document.createElement("div");
            btnHide.innerHTML = "🐾"; // 🗶 👞
            btnHide.setAttribute('role', 'button');
            btnHide.setAttribute('class', 'BIDenb LmajHd n8JNod');
            btnHide.setAttribute('style', 'cursor:crosshair;');
            btnHide.setAttribute('jsaction', 'click:cOuCgd;'); // to prevent disclosure

            let _= this;
          	btnHide.addEventListener("click", function()
            {
                _.process(elem);
            });

            elem.setAttribute(_.$.idents.ownPostLabel, 'false');

            //elem.parentNode.insertBefore(btnHide, elem.nextSibling);
            elem.appendChild(btnHide);
            _.dbg('Added control for: ', elem);
        }

        addControls(elements)
        {
            let _= this;
            for(let elem of elements)
            {
                if(!('getAttribute' in elem) 
                    || elem.getAttribute('jscontroller') != _.$.idents.rcvpost)
                {
                    continue;
                }
                
                let found = _.getElem(_.$.idents.headerPost, elem);
                if(_.isNull(found)) {
                    continue;
                }

                if(!found.hasAttribute(_.$.idents.ownPostLabel)) {
                    _.addControl(found);
                }
            }
        }

        setPostView(scope, hide)
        {
            scope.setAttribute(this.$.idents.ownPostLabel, hide);

            let parent      = scope.parentNode;
            let content     = parent.nextSibling;
            let blinks      = content.nextSibling;
            let comments    = blinks.nextSibling;

            let [display, opacity] = ['', ''];
            if(hide) {
                display = 'display:none;';
                opacity = 'opacity:0.4;'
            }

            parent.setAttribute('style', opacity);
            content.setAttribute('style', display);
            blinks.setAttribute('style', display);
            comments.setAttribute('style', display);
        }
        
        getElem(query, scope, all)
        {
            if(!query) {
                return null;
            }
            
            if(this.isNull(scope)) {
                this.dbg('Use default scope');
                scope = document;
            }

            if(all) {
                return scope.querySelectorAll(query);
            }
            return scope.querySelector(query);
        }
        
        getLastElem(query, scope)
        {
            let elems = this.getElem(query, scope, true);
            if(!('length' in elems) || elems.length < 1) {
                return null;
            }
            return elems[elems.length - 1];
        }

        dbg(msg, ...args)
        {
            if(!this.$.debug) {
                return;
            }

            let stamp = new Date().toISOString().substr(11, 12) + '] ';
            if(this.isNull(args) || args.length < 1) {
                console.log(stamp + msg);
            }
            else {
                console.log(stamp + msg, args);
            }
        }

        azgen(len)
        {
            len = len || 0;

            let code = '';
            let [min, max] = [97, 122]; // a-z

            for(let i = 0; i < len; ++i) {
                code += String.fromCharCode(Math.random() * (max - min) + min);
            }
            return code;
        }

        isNull(val)
        {
            return val == null || val === undefined;
        }
    }

    class GpmException
    {
        constructor(message, arg)
        {
            this.message    = message;
            this.arg        = arg;
        }
    }

    class GpmNullException extends GpmException
    {
        constructor(...args)
        {
            super("'" + args + "' cannot be null.", null);
        }
    }

    // ...

    new GpExMenu(true);

})();
