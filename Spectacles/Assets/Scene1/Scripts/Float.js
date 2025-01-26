// @input float speed = 1.0
// @input float amplitude = 0.1

var startPosition = script.getTransform().getWorldPosition();

function onUpdate(eventData) {
    var time = getTime() * script.speed;
    var offset = Math.sin(time) * script.amplitude;
    var newPosition = startPosition.add(new vec3(0, offset, 0));
    script.getTransform().setWorldPosition(newPosition);
}

script.createEvent("UpdateEvent").bind(onUpdate);