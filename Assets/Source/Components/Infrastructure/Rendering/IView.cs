﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IView
{
	void Link(GameEntity entity);
	void Update();
}