<?php

/*
  * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
  * Distributed under the Boost Software License, Version 1.0
  * (see accompanying file LICENSE or a copy at http://www.boost.org/LICENSE_1_0.txt)
 */

namespace reg\utility\jsonrender;

interface ITypeOutput
{
    const STREAM    = 0;
    const FILE      = 1;
}

interface ITTFStyle
{
    const NORMAL        = 0;
    const BOLD          = 1;
    const ITALIC        = 2;
    const BOLDITALIC    = 3;
    const UNDERLINE     = 4;
    const SHADOW        = 5;
}

class TTFParam
{
    /**
     * RGB, background
     * @var array 
     */
    public $background  = null;
    /**
     * RGB, foreground
     * @var array 
     */
    public $foreground  = null;
    /**
     * transparent
     * @var boolean 
     */
    public $alpha       = false;
    /**
     * TODO: load helper
     * @var string
     */
    public $name    = "fonts/TrueType/Courier_New/normal.ttf";
    public $size    = 9;
    
    /**
     * different font style
     * @var ITTFStyle 
     */
    public /*ITTFStyle*/ $style = ITTFStyle::NORMAL;  

    public function __construct(array $color = array(0, 0, 0), array $bgColor = array(255, 255, 255), $alpha = false)
    {
        $this->foreground = $color;
        $this->background = $bgColor;
        $this->alpha      = $alpha;
    }
}

class JsonData
{
    /**
     * multiline split symbol
     */
    const SYMBSPLIT = "\n";    
    /**
     * formated data
     * @var string
     */
    private $_data;
    /**
     * @param Formatter $tpl - template
     * @param string $url
     */
    public function fromUrl(Formatter $tpl, $url)
    {
        $this->fromString($tpl, file_get_contents($url));
    }
    
    public function fromString(Formatter $tpl, $text)
    {
        $o = json_decode($text, true);
        
        $ret = array();
        foreach($tpl->jsonElems as $elem){
            $val = $o; // :(
            for($i = 0, $n = count($elem) - 1; $i < $n; ++$i){
                $val = $val[$elem[$i]];
            }
            
            $l = $this->callbackParser($elem[$i]);
            if($l instanceof WrapperCallback){
                $ret[] = call_user_func($l->function, $val[$l->level]);
            }
            else{
                $ret[] = $val[$l];
            }
        }
        $this->_data = $this->formatData($tpl->caption, $ret);
    }
    
    /**
     * TODO: external implementation for advanced formatting
     */
    public function renderSimple(TTFParam $ttf)
    {
        if(!file_exists($ttf->name)){
            throw new \Exception("font not found: " . $ttf->name);
        }
        
        //calculates the bounding box in pixels
        $box = imagettfbbox($ttf->size, 0, $ttf->name, $this->_data);
        
        $top    = abs($box[7]);
        $left   = abs($box[0]);
        $w      = ($box[2] - $box[0]) + $left + 4; // maybe a bug imagettfbbox: various delta width on 7pt and e.g.: 26pt for GDv2
        $h      = ($box[1] - $box[7]);
        
        $img        = imagecreatetruecolor($w, $h);
        $background = imagecolorallocate($img, $ttf->background[0], $ttf->background[1], $ttf->background[2]);
        $foreground = imagecolorallocate($img, $ttf->foreground[0], $ttf->foreground[1], $ttf->foreground[2]);
        
        //background transparent
        if($ttf->alpha){
            imagecolortransparent($img, $background);
        }        

        imagefilledrectangle($img, 0, 0, $w, $h, $background);
        imagettftext($img, $ttf->size, 0, $left, $top, $foreground, $ttf->name, $this->_data);
        
        header('Cache-Control: no-cache, must-revalidate');
        header('Expires: Sat, 14 Jul 1990 01:00:00 GMT');        
        header('Content-Type: image/png');
        imagepng($img);
        imagedestroy($img);
    }

    public function getData()
    {
        return $this->_data;
    }
    
    /**
     * alternative sprintf with array values
     * ...only %s placeholder
     * @param string $str
     * @param array $values
     * @return string
     */
    protected function formatData($str, array $values)
    {
        return preg_replace_callback("#%s#i", 
                  function($m) use(&$values){
                    return array_shift($values);
                  }
                , $str);
    }
    
    /**
     * @param string $str - last level
     * @return mixed - WrapperCallback / string
     */
    protected function callbackParser($str)
    {
        preg_match("#([^\($]+)(?:\(([^\)]+))?#", $str, $match);
        if(isset($match[2])){
           return new WrapperCallback($match[1], $match[2]);
        }
        return $match[1];
    }
}

/**
 * helper object
 */
class Formatter
{
    /**
     * with placeholders
     * @var string
     */
    public /*string*/ $caption;
    
    /**
     * path to elements
     * @var array
     */
    public $jsonElems = array();
    
    /**
     * @param string $caption
     * @param array $jsonElems - formats: 'elem1.elem2.elem3', ..
     */
    public function __construct($caption, array $jsonElems)
    {
        $this->caption      = $caption;
        $this->jsonElems    = $this->_epath($jsonElems);
    }
    
    private function _epath(&$elems)
    {
        $ret = array();
        foreach($elems as $elem){
            $ret[] = explode('.', $elem);
        }
        return $ret;
    }
}

class WrapperCallback
{
    /**  @var string */
    public $level;
    /** @var string */
    public $function;
    
    public function __construct($level, $function)
    {
        $this->level    = $level;
        $this->function = __NAMESPACE__ . '\\' . $function;
    }
}