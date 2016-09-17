<?php

/*
  * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
  * Distributed under the Boost Software License, Version 1.0
  * (see accompanying file LICENSE or a copy at http://www.boost.org/LICENSE_1_0.txt)
 */

namespace reg\graphics\_2d\text\imagestring;

require_once 'imagestring.php';

/* pagination render - ALL into files */
/*
    $render = new TextRender('Courier_New', 90, 60);
    $render->setFnameForRender('textout');
    //$render->splitHeavy = true;
    $pages  = $render->renderIntoFile("that this notice be translated ... long long text ...", new GDFColor(array(190, 209, 214)));
    //$pages  = $render->renderIntoFile(TextRender::utf8To1251(""));
*/

/* stream by part */
/*
    $render = new TextRender('Courier_New', 90, 60);
    $pages  = $render->renderIntoStreamPart(IParts::FIRST, "that this notice be translated ... long long text ...", new GDFColor(array(120, 109, 114)));
*/

/* simple */
$render = new TextRender('Courier_New', 320, 140);
$pages  = $render->renderIntoStream("Hello,<br>&nbsp;&nbsp;entry.<b>reg</b>@gmail.com", new GDFColor(array(120, 109, 114)));


