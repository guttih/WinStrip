#!/usr/bin/env bash
set -euo pipefail

project_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
test_build_dir="$(mktemp -d "${TMPDIR:-/tmp}/esp32strip-tests.XXXXXX")"
trap 'rm -rf -- "$test_build_dir"' EXIT

test_compile_flags=()
if [[ "${HOST_TEST_SANITIZERS:-0}" == 1 ]]; then
    test_compile_flags+=( -fsanitize=address,undefined -fno-omit-frame-pointer )
fi

"${CXX:-g++}" -std=c++11 -Wall -Wextra -Werror -pedantic -g \
    "${test_compile_flags[@]}" \
    -I"$project_dir/test/host" -I"$project_dir/lib/StripReader" \
    "$project_dir/lib/StripReader/SerialReader.cpp" \
    "$project_dir/test/host/test_serial_reader.cpp" \
    -o "$test_build_dir/test_serial_reader"

"$test_build_dir/test_serial_reader"

for test_name in bluetooth_session animation_clock; do
    "${CXX:-g++}" -std=c++11 -Wall -Wextra -Werror -pedantic -g \
        "${test_compile_flags[@]}" \
        -I"$project_dir/lib/BluetoothTransport" -I"$project_dir/lib/StripHelper" \
        "$project_dir/test/host/test_$test_name.cpp" \
        -o "$test_build_dir/test_$test_name"
    "$test_build_dir/test_$test_name"
done
