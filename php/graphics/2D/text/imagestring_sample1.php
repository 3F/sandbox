<?php

/*
  * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
  * Distributed under the Boost Software License, Version 1.0
  * (see accompanying file LICENSE or a copy at http://www.boost.org/LICENSE_1_0.txt)
 */

namespace reg\graphics\_2d\text\imagestring;

require_once 'imagestring.php';

/* pagination render */
$render = new TextRender('Courier_New', 90, 60);
$render->setFnameForRender('textout');
$pages  = $render->renderIntoFile("that this notice be translated ... long long text ...", array(190, 209, 214));
//$pages  = $render->renderIntoFile(TextRender::utf8To1251(""));